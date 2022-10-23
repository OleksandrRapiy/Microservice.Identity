using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microservice.Identity.Application.Dtos;
using Microservice.Identity.Domain.Entities;
using Microservice.Identity.Domain.Models;
using Microservice.Identity.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Identity.Application.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CreateUserCommandHandler(UserManager<UserEntity> userManager, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new UserEntity(request.Email, request.UserName, request.FirstName, request.LastName);

            var role = await _context.Roles.FirstAsync(r => r.Name == IdentityRoles.User,
                    cancellationToken: cancellationToken);

            var result = await _userManager.CreateAsync(user, request.Password);

            user.UserRoles.Add(new UserRoleEntity(user, role));

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return _mapper.Map<UserDto>(user);

        }
    }
}
