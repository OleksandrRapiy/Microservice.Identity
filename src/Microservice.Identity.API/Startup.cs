using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microservice.Identity.Application.AutoMapper;
using Microservice.Identity.Application.Commands;
using Microservice.Identity.Application.Configurations;
using Microservice.Identity.Application.Interfaces.Seeders;
using Microservice.Identity.Domain.Entities;
using Microservice.Identity.Infrastructure.Services;
using Microservice.Identity.Persistence.Context;
using Microservice.Identity.Persistence.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using static Microservice.Identity.Application.Configurations.IdentityServerConfigurations;

namespace Microservice.Identity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {

            #region DataBase

            services.AddScoped<ISeeder, UserSeeder>();

            var postgreSqlDbConnection = Configuration.GetSection("DbConnection").Value;
            if (postgreSqlDbConnection is null) throw new ArgumentNullException(nameof(postgreSqlDbConnection));

            services.AddDbContext<ApplicationDbContext>(config =>
            {
                config.UseNpgsql(postgreSqlDbConnection, options => options.EnableRetryOnFailure());
            });

            #endregion

            services.AddControllers();

            services.AddHttpClient("IdentityToken",
                 httpClient => { httpClient.BaseAddress = new Uri($"{Configuration["Identity:Authority"]}/connect/token"); });

            services.AddIdentity<UserEntity, RoleEntity>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

            })
            .AddRoles<RoleEntity>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserManager<UserManager<UserEntity>>()
            .AddDefaultTokenProviders()
            .Services
            .AddIdentityServer(options =>
            {
                options.IssuerUri = new Uri(Configuration["Identity:Authority"]).Host;
            })
            .AddDeveloperSigningCredential()
            .AddAspNetIdentity<UserEntity>()
            .AddProfileService<ProfileService>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseNpgsql(postgreSqlDbConnection,
                        sql =>
                        {
                            sql.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly
                                .GetName().Name);
                        });
            })
            .AddInMemoryApiResources(ApiResources)
            .AddInMemoryClients(GetClients)
            .AddInMemoryApiScopes(ApiScopes);


            // Authorization 
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = Configuration["Identity:Authority"];
                options.ApiName = Configuration["Identity:ApiName"];

                options.TokenRetriever = request =>
                {
                    request.Cookies.TryGetValue("access_token", out var cookiesToken);
                    if (request.Headers.TryGetValue("Authorization", out var headerToken))
                        headerToken = headerToken.ToString().Split(' ').Last();

                    var resp = cookiesToken ?? headerToken;

                    return resp;
                };
            });
            // You create policy if token have required scope
            services.AddAuthorization(options =>
            {
                options.AddPolicy("microservice.identity.api",
                    policy => policy.RequireScope("microservice.identity.api"));
            });

            services.AddMediatR(typeof(CreateUserCommand).Assembly,
                    typeof(CreateUserCommandHandler).Assembly);

            #region Services

            services.AddScoped<ISeeder, UserSeeder>();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            #endregion

            #region Swagger 

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Microservice.Identity.API", Version = "v1" });
                options.AddServer(new OpenApiServer()
                {
                    Url = "https://localhost:5001"
                });


                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"https://localhost:5001/connect/token"),
                            AuthorizationUrl = new Uri($"https://localhost:5001/connect/authorize"),
                        },
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "oauth2",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservice.Identity.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseIdentityServer();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
