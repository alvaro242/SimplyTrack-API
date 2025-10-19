using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplyTrack.Api.Models.DTOs;
using SimplyTrack.Api.Services;
using System.Security.Claims;

namespace SimplyTrack.Api.Controllers
{
    /// <summary>
    /// Dashboard controller providing overview data for the fitness tracking application
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    [Tags("Dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public DashboardController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        /// <summary>
        /// Get dashboard overview of user's exercises with recent session data
        /// </summary>
        /// <param name="limit">Maximum number of exercises to return (default: 100)</param>
        /// <returns>List of exercises with dashboard summary information</returns>
        /// <response code="200">Dashboard exercises retrieved successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        [HttpGet("exercises")]
        [ProducesResponseType(typeof(List<ExerciseDashboardItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ExerciseDashboardItemDto>>> GetDashboardExercises([FromQuery] int limit = 100)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var exercises = await _exerciseService.GetUserExercisesAsync(userId, null, true);
            
            // Limit results
            if (limit > 0)
                exercises = exercises.Take(limit).ToList();

            return Ok(exercises);
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        }
    }
}