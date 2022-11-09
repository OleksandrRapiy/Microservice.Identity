using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.EntityFramework.Entities;
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

            if (!IdentityServerConfigurations.GetClients.Any(x => x.ClientId == request.ClientId && x.ClientSecrets.Any(x => x.Value == request.ClientSecret.ToSha256())))
                throw new Exception("Invalid client id or clinet secret");

            var tokenClient = new TokenClient(_httpClient, new TokenClientOptions()
            {
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
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
