using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using MediatR;
using Microservice.Identity.Application.Commands;
using Microservice.Identity.Application.Configurations;
using Microservice.Identity.Application.Dtos;
using Microservice.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Microservice.Identity.Infrastructure.Handlers.CommandHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
    {
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly HttpClient _httpClient;

        public LoginCommandHandler(SignInManager<UserEntity> signInManager, IHttpClientFactory httpClientFactory)
        {
            _signInManager = signInManager;
            _httpClient = httpClientFactory.CreateClient("IdentityToken");
        }

        public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(request.Username);
            if (user == null)
                throw new Exception("User not found");

            if (await _signInManager.CheckPasswordSignInAsync(user, request.Password, true) != SignInResult.Success)
                throw new Exception("Invalid credentials");

            var tokenClient = new TokenClient(_httpClient, new TokenClientOptions()
            {
                ClientId = IdentityServerConfigurations.InternalClient.ClientId,
                ClientSecret = IdentityServerConfigurations.InternalClientSecret,
            });

            var token = await tokenClient.RequestPasswordTokenAsync(
                user.UserName, request.Password,
                cancellationToken: cancellationToken);

            if (token.IsError)
                throw new Exception(token.Error);

            return new TokenDto(token.AccessToken, token.RefreshToken, token.ExpiresIn);
        }
    }
}
