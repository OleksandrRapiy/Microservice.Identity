using AutoMapper;
using Microservice.Identity.Application.Dtos;
using Microservice.Identity.Domain.Entities;
using Microservice.Identity.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Identity.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IMapper _mapper;

        public UsersController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet]
        //[Authorize(Roles = IdentityRoles.Admin)]
        public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);

            if (!users.Any())
                NotFound();

            return Ok(_mapper.Map<List<UserDto>>(users));
        }


        [HttpGet("{id:long:min(1)}")]
        //[Authorize(Roles = IdentityRoles.Admin)]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user == null)
                NotFound();

            return Ok(_mapper.Map<UserDto>(user));
        }
    }
}
