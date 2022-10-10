using AutoMapper;
using Microservice.Identity.Application.Dtos;
using Microservice.Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Identity.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(CancellationToken cancellationToken)
        {
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegitsterAsync(CancellationToken cancellationToken)
        {
            return Ok();
        }


        [Authorize]
        [HttpGet("personal-info")]
        public async Task<IActionResult> GetPersonalInfoAsync(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            return Ok(_mapper.Map<UserDto>(user));
        }
    }
}
