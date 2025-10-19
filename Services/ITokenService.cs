using SimplyTrack.Api.Models;

namespace SimplyTrack.Api.Services
{
    public interface ITokenService
    {
        Task<(string accessToken, RefreshToken refreshToken)> CreateTokensAsync(ApplicationUser user, string ipAddress);
        string GenerateAccessToken(ApplicationUser user);
        RefreshToken GenerateRefreshToken(string ipAddress, TimeSpan lifetime);
    }
}