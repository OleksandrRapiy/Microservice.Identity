﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microservice.Identity.Application.Commands;
using Microservice.Identity.Application.Dtos;
using Microservice.Identity.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Identity.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AccountsController(
            UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager,
            IMapper mapper,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _mediator = mediator;
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            Response.Cookies.Append("access_token", result.AccessToken, new()
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddSeconds(result.ExpiresIn),

            });
            Response.Cookies.Append("refresh_token", result.RefreshToken, new()
            {
                HttpOnly = true,
            });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(command, cancellationToken));


        [Authorize]
        [HttpGet("personal-info")]
        public async Task<IActionResult> GetPersonalInfoAsync(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            return Ok(_mapper.Map<UserDto>(user));
        }
    }
}
