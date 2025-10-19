using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimplyTrack.Api.Models;
using SimplyTrack.Api.Models.DTOs;
using SimplyTrack.Api.Services;
using System.Security.Claims;

namespace SimplyTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new ErrorDto { Code = "USER_EXISTS", Message = "User with this email already exists" });
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ErrorDto 
                { 
                    Code = "REGISTRATION_FAILED", 
                    Message = string.Join(", ", result.Errors.Select(e => e.Description)) 
                });
            }

            // Generate tokens
            var ipAddress = GetIpAddress();
            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user, ipAddress);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return CreatedAtAction(nameof(Register), response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new ErrorDto { Code = "INVALID_CREDENTIALS", Message = "Invalid email or password" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ErrorDto { Code = "INVALID_CREDENTIALS", Message = "Invalid email or password" });
            }

            // Revoke existing refresh tokens for this user (optional security measure)
            // await RevokeAllUserRefreshTokensAsync(user.Id);

            // Generate tokens
            var ipAddress = GetIpAddress();
            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user, ipAddress);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var ipAddress = GetIpAddress();
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                return Unauthorized(new ErrorDto { Code = "INVALID_TOKEN", Message = "Invalid or expired refresh token" });
            }

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                return Unauthorized(new ErrorDto { Code = "USER_NOT_FOUND", Message = "User not found" });
            }

            // Revoke the old refresh token and create new tokens
            var (accessToken, newRefreshToken) = await _tokenService.CreateTokensAsync(user, ipAddress);
            await _refreshTokenRepository.RevokeAsync(refreshToken, newRefreshToken.Token, ipAddress);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout([FromBody] RefreshTokenRequestDto? request = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // If refresh token provided, revoke it specifically
            if (request?.RefreshToken != null)
            {
                var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
                if (refreshToken != null && refreshToken.UserId == userId)
                {
                    var ipAddress = GetIpAddress();
                    await _refreshTokenRepository.RevokeAsync(refreshToken, string.Empty, ipAddress);
                }
            }

            // Optional: Revoke all refresh tokens for the user
            // await RevokeAllUserRefreshTokensAsync(userId);

            return NoContent();
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<ActionResult> RevokeToken([FromBody] RefreshTokenRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.UserId != userId)
            {
                return BadRequest(new ErrorDto { Code = "INVALID_TOKEN", Message = "Invalid refresh token" });
            }

            var ipAddress = GetIpAddress();
            await _refreshTokenRepository.RevokeAsync(refreshToken, string.Empty, ipAddress);

            return NoContent();
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        // Helper method for revoking all user tokens (if needed)
        // private async Task RevokeAllUserRefreshTokensAsync(string userId)
        // {
        //     // This would require additional repository method
        //     // var userTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(userId);
        //     // foreach (var token in userTokens)
        //     // {
        //     //     await _refreshTokenRepository.RevokeAsync(token, string.Empty, GetIpAddress());
        //     // }
        // }
    }
}