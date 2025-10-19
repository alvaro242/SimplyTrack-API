using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimplyTrack.Api.Models;
using SimplyTrack.Api.Models.DTOs;
using System.Security.Claims;

namespace SimplyTrack.Api.Controllers
{
    /// <summary>
    /// User management controller for retrieving and updating user profile information
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    [Tags("User Management")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Get current user's profile information
        /// </summary>
        /// <returns>Current user's profile data</returns>
        /// <response code="200">User profile retrieved successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">User not found</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = DateTime.UtcNow // You might want to add CreatedAt to ApplicationUser
            };

            return Ok(userDto);
        }

        /// <summary>
        /// Update current user's profile information
        /// </summary>
        /// <param name="request">User profile update data</param>
        /// <returns>Updated user profile</returns>
        /// <response code="200">User profile updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">User not found</response>
        [HttpPatch("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> UpdateMe([FromBody] UpdateUserDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.FirstName))
                user.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.LastName))
                user.LastName = request.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ErrorDto 
                { 
                    Code = "UPDATE_FAILED", 
                    Message = string.Join(", ", result.Errors.Select(e => e.Description)) 
                });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = DateTime.UtcNow
            };

            return Ok(userDto);
        }

        [HttpDelete("me")]
        public async Task<ActionResult> DeleteMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ErrorDto 
                { 
                    Code = "DELETE_FAILED", 
                    Message = string.Join(", ", result.Errors.Select(e => e.Description)) 
                });
            }

            // Note: You might also want to clean up related data (exercises, sessions, etc.)
            // and revoke all refresh tokens for this user

            return NoContent();
        }
    }
}