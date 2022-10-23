using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microservice.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Identity.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<UserEntity> _userManager;

        public ProfileService(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subject);
            var roles = await _userManager.GetRolesAsync(user);

            var tokenClaims = new List<Claim>();
            tokenClaims.AddRange(roles.Select(role => new Claim("Role", role)).ToList());
            tokenClaims.Add(new Claim("UserName", user.UserName));
            tokenClaims.Add(new Claim(JwtClaimTypes.Email, user.Email));
            tokenClaims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
            tokenClaims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));

            context.IssuedClaims.AddRange(tokenClaims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
