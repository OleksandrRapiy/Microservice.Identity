using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Microservice.Identity.Domain.Entities
{
    public class UserEntity : IdentityUser<long>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}
