using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Identity.Domain.Entities
{
    public class UserRoleEntity : IdentityUserRole<long>
    {
        public UserEntity User { get; set; }
        public RoleEntity Role { get; set; }

        public UserRoleEntity()
        {
        }

        public UserRoleEntity(UserEntity user, RoleEntity role)
        {
            User = user;
            Role = role;
        }
    }
}
