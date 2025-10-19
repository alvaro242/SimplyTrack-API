using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplyTrack.Api.Models.DTOs;
using SimplyTrack.Api.Services;
using System.Security.Claims;

namespace SimplyTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public DashboardController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet("exercises")]
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
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}