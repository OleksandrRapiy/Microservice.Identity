using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Identity.Domain.Entities
{
    public class RoleEntity : IdentityRole<long>
    {
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();

        public RoleEntity() { }

        public RoleEntity(string roleName) : base(roleName)
        {
        }
    }
}
