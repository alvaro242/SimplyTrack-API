using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ApplicationDBContext.Api.Data;
using SimplyTrack.Api.Models;

namespace SimplyTrack.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _config = config;
            _userManager = userManager;
            _db = db;
        }

        public async Task<(string accessToken, RefreshToken refreshToken)> CreateTokensAsync(ApplicationUser user, string ipAddress)
        {
            var accessToken = GenerateAccessToken(user);

            var refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"]);
            var refreshToken = GenerateRefreshToken(ipAddress, TimeSpan.FromDays(refreshDays));
            refreshToken.UserId = user.Id;

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        public string GenerateAccessToken(ApplicationUser user)
        {
            // Use same pattern as Program.cs - environment variables override appsettings.json
            string jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? _config["Jwt:Key"];
            string jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? _config["Jwt:Issuer"];
            string jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? _config["Jwt:Audience"];
            string accessTokenMinutesStr = Environment.GetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES") ?? _config["Jwt:AccessTokenExpirationMinutes"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT_KEY must be provided via environment variable or appsettings.json.");
            }

            if (!double.TryParse(accessTokenMinutesStr, out var accessTokenMinutes))
                accessTokenMinutes = 15;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("given_name", user.FirstName ?? ""),
                new Claim("family_name", user.LastName ?? "")
            };

            var identityClaims = new ClaimsIdentity(claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddMinutes(accessTokenMinutes),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string ipAddress, TimeSpan lifetime)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);

            return new RefreshToken
            {
                Token = token,
                Expires = DateTime.UtcNow.Add(lifetime),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}