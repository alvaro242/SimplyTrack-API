using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplyTrack.Api.Models.DTOs;
using SimplyTrack.Api.Services;
using System.Security.Claims;

namespace SimplyTrack.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly ISetService _setService;

        public SessionsController(ISessionService sessionService, ISetService setService)
        {
            _sessionService = sessionService;
            _setService = setService;
        }

        [HttpPost("api/exercises/{exerciseId}/sessions")]
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

        [HttpGet("api/exercises/{exerciseId}/sessions")]
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

        [HttpGet("api/sessions/{sessionId}")]
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

        [HttpDelete("api/sessions/{sessionId}")]
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

        [HttpPost("api/sessions/{sessionId}/sets")]
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
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}