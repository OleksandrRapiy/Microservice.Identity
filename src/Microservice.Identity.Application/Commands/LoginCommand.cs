using System.ComponentModel.DataAnnotations;
using MediatR;
using Microservice.Identity.Application.Dtos;

namespace Microservice.Identity.Application.Commands
{
    public class LoginCommand : IRequest<TokenDto>
    {
        [Required]
        //[EmailAddress(ErrorMessage = "Incorrect email address")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password should not be empty")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
