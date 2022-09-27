using Microservice.Identity.Application.Interfaces.Seeders;
using Microservice.Identity.Domain.Entities;
using Microservice.Identity.Domain.Models;
using Microservice.Identity.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Identity.Persistence.Seeders
{
    public class UserSeeder : ISeeder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<RoleEntity> _roleManager;
        private readonly UserManager<UserEntity> _userManager;

        public UserSeeder(ApplicationDbContext dbContext, RoleManager<RoleEntity> roleManager, UserManager<UserEntity> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await SeedRoleAsync(IdentityRoles.User);
            await SeedRoleAsync(IdentityRoles.Admin);
            await SeedAdminAsync();
        }

        private async Task SeedRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new RoleEntity(roleName));

                if (!result.Succeeded)
                    throw new Exception($"Error on adding {roleName} role.");
            }
        }

        private async Task SeedAdminAsync()
        {
            if (await _userManager.Users.AllAsync(us => us.Email != "admin@platform.com"))
            {
                var admin = new UserEntity
                {
                    Email = "admin@mail.com",
                    UserName = "mainadmin",
                    EmailConfirmed = true,
                    PhoneNumber = "+10000000000",
                    PhoneNumberConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Admin"
                };

                await _userManager.CreateAsync(admin, "AdminPass123$$");
                await _userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
            }
        }
    }
}
