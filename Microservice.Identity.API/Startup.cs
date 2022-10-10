using IdentityServer4.AccessTokenValidation;
using Microservice.Identity.Application.AutoMapper;
using Microservice.Identity.Application.Configurations;
using Microservice.Identity.Application.Interfaces.Seeders;
using Microservice.Identity.Domain.Entities;
using Microservice.Identity.Infrastructure.Services;
using Microservice.Identity.Persistence.Context;
using Microservice.Identity.Persistence.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microservice.Identity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("Microservice.Identity"));

            services.AddIdentity<UserEntity, RoleEntity>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
            }).AddRoles<RoleEntity>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserManager<UserManager<UserEntity>>()
            .AddDefaultTokenProviders()
            .Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "https://localhost:5001";
                options.ApiSecret = IdentityServerConfigurations.InternalClientSecret;
                
                options.EnableCaching = true;

                options.TokenRetriever = request =>
                {
                    request.Cookies.TryGetValue("access_token", out var cookiesToken);
                    if (request.Headers.TryGetValue("Authorization", out var headerToken))
                        headerToken = headerToken.ToString().Split(' ').Last();

                    return cookiesToken ?? headerToken;
                };
            })
            .Services
            .AddIdentityServer(options =>
            {
                options.IssuerUri = new Uri("https://localhost:5001").Host;
            })
            .AddAspNetIdentity<UserEntity>()
            .AddProfileService<ProfileService>()
            .AddInMemoryClients(IdentityServerConfigurations.GetClients)
            .AddInMemoryApiScopes(IdentityServerConfigurations.ApiScopes)
            ;


            #region Services

            services.AddScoped<ISeeder, UserSeeder>();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            
            #endregion

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

            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
