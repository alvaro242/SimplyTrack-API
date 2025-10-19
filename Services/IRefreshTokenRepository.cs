using SimplyTrack.Api.Models;

namespace SimplyTrack.Api.Services
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task RevokeAsync(RefreshToken token, string replacedByToken, string ipAddress);
        Task AddAsync(RefreshToken token);
    }
}