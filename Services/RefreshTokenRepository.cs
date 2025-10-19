using Microsoft.EntityFrameworkCore;
using ApplicationDBContext.Api.Data;
using SimplyTrack.Api.Models;

namespace SimplyTrack.Api.Services
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _db;
        public RefreshTokenRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(RefreshToken token)
        {
            _db.RefreshTokens.Add(token);
            await _db.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task RevokeAsync(RefreshToken token, string replacedByToken, string ipAddress)
        {
            if (token == null) return;
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReplacedByToken = replacedByToken;
            _db.RefreshTokens.Update(token);
            await _db.SaveChangesAsync();
        }
    }
}