using MediatR;
using Microservice.Identity.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Microservice.Identity.Application.Commands
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        [Required]
        public string UserName { get; set; }        
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Incorrect email address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
