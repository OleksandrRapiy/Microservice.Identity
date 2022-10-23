using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Microservice.Identity.Domain.Entities
{
    public class UserEntity : IdentityUser<long>
    {
        public UserEntity(string email, string username, string firstName, string lastName)
        {
            Email = email;
            UserName = username;
            FirstName = firstName;
            LastName = lastName;
        }
        public UserEntity()
        { }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}
