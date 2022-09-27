﻿using Microservice.Identity.Domain.Entities;
using Microservice.Identity.Persistence.EntityConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Identity.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<UserEntity, RoleEntity, long, IdentityUserClaim<long>,
        UserRoleEntity, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public override DbSet<UserEntity> Users { get; set; }
        public override DbSet<RoleEntity> Roles { get; set; }
        public override DbSet<UserRoleEntity> UserRoles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserEntityConfiguration());
        }
    }
}
