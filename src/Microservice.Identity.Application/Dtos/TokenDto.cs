namespace Microservice.Identity.Application.Dtos
{
    public class TokenDto
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public long ExpiresIn { get; }

        public TokenDto(string accessToken, string refreshToken, long expiresIn)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
    }
}
