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
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExerciseDashboardItemDto>>> GetExercises(
            [FromQuery] string? q = null,
            [FromQuery] bool includeLastSession = true)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var exercises = await _exerciseService.GetUserExercisesAsync(userId, q, includeLastSession);
            return Ok(exercises);
        }

        [HttpPost]
        public async Task<ActionResult<ExerciseDto>> CreateExercise([FromBody] CreateExerciseRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var exercise = await _exerciseService.CreateExerciseAsync(request, userId);

            var exerciseDto = new ExerciseDto
            {
                Id = exercise.Id,
                UserId = exercise.UserId,
                Name = exercise.Name,
                Notes = exercise.Description,
                CreatedAt = exercise.CreatedAt
            };

            return CreatedAtAction(nameof(GetExercise), new { exerciseId = exercise.Id }, exerciseDto);
        }

        [HttpGet("{exerciseId}")]
        public async Task<ActionResult<ExerciseDetailDto>> GetExercise(string exerciseId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var exercise = await _exerciseService.GetExerciseDetailAsync(exerciseId, userId);
            if (exercise == null)
                return NotFound();

            return Ok(exercise);
        }

        [HttpPatch("{exerciseId}")]
        public async Task<ActionResult<ExerciseDto>> UpdateExercise(string exerciseId, [FromBody] UpdateExerciseDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var exercise = await _exerciseService.UpdateExerciseAsync(exerciseId, request, userId);
            if (exercise == null)
                return NotFound();

            var exerciseDto = new ExerciseDto
            {
                Id = exercise.Id,
                UserId = exercise.UserId,
                Name = exercise.Name,
                Notes = exercise.Description,
                CreatedAt = exercise.CreatedAt
            };

            return Ok(exerciseDto);
        }

        [HttpDelete("{exerciseId}")]
        public async Task<ActionResult> DeleteExercise(string exerciseId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _exerciseService.DeleteExerciseAsync(exerciseId, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{exerciseId}/history")]
        public async Task<ActionResult<List<SessionWithSetsDto>>> GetExerciseHistory(
            string exerciseId,
            [FromQuery] int limit = 100,
            [FromQuery] int offset = 0)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // This would require a new service method
            // For now, return empty list as placeholder
            return Ok(new List<SessionWithSetsDto>());
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}