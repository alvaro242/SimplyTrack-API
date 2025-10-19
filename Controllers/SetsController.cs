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
    public class SetsController : ControllerBase
    {
        private readonly ISetService _setService;

        public SetsController(ISetService setService)
        {
            _setService = setService;
        }

        [HttpPatch("{setId}")]
        public async Task<ActionResult<SetDto>> UpdateSet(string setId, [FromBody] UpdateSetDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var set = await _setService.UpdateSetAsync(setId, request, userId);
            if (set == null)
                return NotFound();

            var setDto = new SetDto
            {
                Id = set.Id,
                SessionId = set.SessionId,
                Reps = set.Reps,
                Weight = set.Weight,
                CreatedAt = set.CreatedAt
            };

            return Ok(setDto);
        }

        [HttpDelete("{setId}")]
        public async Task<ActionResult> DeleteSet(string setId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _setService.DeleteSetAsync(setId, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}