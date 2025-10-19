using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplyTrack.Api.Models.DTOs;
using SimplyTrack.Api.Services;
using System.Security.Claims;

namespace SimplyTrack.Api.Controllers
{
    /// <summary>
    /// Exercise management controller for CRUD operations on fitness exercises
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    [Tags("Exercise Management")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        /// <summary>
        /// Get all exercises for the current user with optional search and session data
        /// </summary>
        /// <param name="q">Optional search query to filter exercises by name</param>
        /// <param name="includeLastSession">Include information about the last workout session</param>
        /// <returns>List of user's exercises</returns>
        /// <response code="200">Exercises retrieved successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ExerciseDashboardItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Create a new exercise for the current user
        /// </summary>
        /// <param name="request">Exercise creation data</param>
        /// <returns>Created exercise details</returns>
        /// <response code="201">Exercise created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        [HttpPost]
        [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Get detailed information about a specific exercise
        /// </summary>
        /// <param name="exerciseId">The unique identifier of the exercise</param>
        /// <returns>Detailed exercise information</returns>
        /// <response code="200">Exercise details retrieved successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Exercise not found or doesn't belong to current user</response>
        [HttpGet("{exerciseId}")]
        [ProducesResponseType(typeof(ExerciseDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Update an existing exercise
        /// </summary>
        /// <param name="exerciseId">The unique identifier of the exercise to update</param>
        /// <param name="request">Exercise update data</param>
        /// <returns>Updated exercise details</returns>
        /// <response code="200">Exercise updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Exercise not found or doesn't belong to current user</response>
        [HttpPatch("{exerciseId}")]
        [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Delete an exercise and all its associated workout sessions
        /// </summary>
        /// <param name="exerciseId">The unique identifier of the exercise to delete</param>
        /// <returns>No content on successful deletion</returns>
        /// <response code="204">Exercise deleted successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Exercise not found or doesn't belong to current user</response>
        [HttpDelete("{exerciseId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Get workout history for a specific exercise
        /// </summary>
        /// <param name="exerciseId">The unique identifier of the exercise</param>
        /// <param name="limit">Maximum number of sessions to return (default: 100)</param>
        /// <param name="offset">Number of sessions to skip for pagination (default: 0)</param>
        /// <returns>List of workout sessions for the exercise</returns>
        /// <response code="200">Exercise history retrieved successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        [HttpGet("{exerciseId}/history")]
        [ProducesResponseType(typeof(List<SessionWithSetsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        }
    }
}