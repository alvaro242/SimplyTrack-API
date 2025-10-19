using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplyTrack.Api.Models.DTOs;
using SimplyTrack.Api.Services;
using System.Security.Claims;

namespace SimplyTrack.Api.Controllers
{
    /// <summary>
    /// Session management controller for workout session operations
    /// </summary>
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Tags("Session Management")]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly ISetService _setService;

        public SessionsController(ISessionService sessionService, ISetService setService)
        {
            _sessionService = sessionService;
            _setService = setService;
        }

        /// <summary>
        /// Create a new workout session for an exercise
        /// </summary>
        /// <param name="exerciseId">The unique identifier of the exercise</param>
        /// <param name="request">Optional session creation data (uses defaults if not provided)</param>
        /// <returns>Created session details</returns>
        /// <response code="201">Session created successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Exercise not found or doesn't belong to current user</response>
        [HttpPost("api/exercises/{exerciseId}/sessions")]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SessionDto>> CreateSession(string exerciseId, [FromBody] CreateSessionRequestDto? request = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            request ??= new CreateSessionRequestDto();

            var session = await _sessionService.CreateSessionAsync(exerciseId, request, userId);
            if (session == null)
                return NotFound("Exercise not found");

            var sessionDto = new SessionDto
            {
                Id = session.Id,
                ExerciseId = session.ExerciseId,
                Date = session.Date,
                CreatedAt = session.CreatedAt,
                TotalWeight = session.TotalWeight,
                TotalReps = session.TotalReps,
                SetsCount = session.SetsCount
            };

            return CreatedAtAction(nameof(GetSession), new { sessionId = session.Id }, sessionDto);
        }

        /// <summary>
        /// Get all workout sessions for a specific exercise
        /// </summary>
        /// <param name="exerciseId">The unique identifier of the exercise</param>
        /// <param name="limit">Maximum number of sessions to return (default: 50)</param>
        /// <param name="offset">Number of sessions to skip for pagination (default: 0)</param>
        /// <param name="from">Start date filter (optional)</param>
        /// <param name="to">End date filter (optional)</param>
        /// <returns>List of workout sessions</returns>
        /// <response code="200">Sessions retrieved successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        [HttpGet("api/exercises/{exerciseId}/sessions")]
        [ProducesResponseType(typeof(List<SessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<SessionDto>>> GetExerciseSessions(
            string exerciseId,
            [FromQuery] int limit = 50,
            [FromQuery] int offset = 0,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var sessions = await _sessionService.GetExerciseSessionsAsync(exerciseId, userId, limit, offset, from, to);
            return Ok(sessions);
        }

        /// <summary>
        /// Get detailed information about a specific workout session
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session</param>
        /// <returns>Detailed session information including all sets</returns>
        /// <response code="200">Session details retrieved successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Session not found or doesn't belong to current user</response>
        [HttpGet("api/sessions/{sessionId}")]
        [ProducesResponseType(typeof(SessionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SessionDetailDto>> GetSession(string sessionId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var session = await _sessionService.GetSessionDetailAsync(sessionId, userId);
            if (session == null)
                return NotFound();

            return Ok(session);
        }

        /// <summary>
        /// Delete a workout session and all its sets
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session to delete</param>
        /// <returns>No content on successful deletion</returns>
        /// <response code="204">Session deleted successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Session not found or doesn't belong to current user</response>
        [HttpDelete("api/sessions/{sessionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSession(string sessionId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _sessionService.DeleteSessionAsync(sessionId, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Add a new set to a workout session
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session</param>
        /// <param name="request">Set creation data including reps, weight, and order</param>
        /// <returns>Created set details</returns>
        /// <response code="201">Set created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Session not found or doesn't belong to current user</response>
        [HttpPost("api/sessions/{sessionId}/sets")]
        [ProducesResponseType(typeof(SetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SetDto>> CreateSet(string sessionId, [FromBody] CreateSetRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var set = await _setService.CreateSetAsync(sessionId, request, userId);
            if (set == null)
                return NotFound("Session not found");

            var setDto = new SetDto
            {
                Id = set.Id,
                SessionId = set.SessionId,
                Reps = set.Reps,
                Weight = set.Weight,
                CreatedAt = set.CreatedAt
            };

            return CreatedAtAction(nameof(GetSession), new { sessionId = set.SessionId }, setDto);
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        }
    }
}