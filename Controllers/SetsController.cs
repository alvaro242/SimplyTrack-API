using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplyTrack.Api.Models.DTOs;
using SimplyTrack.Api.Services;
using System.Security.Claims;

namespace SimplyTrack.Api.Controllers
{
    /// <summary>
    /// Set management controller for individual exercise set operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    [Tags("Set Management")]
    public class SetsController : ControllerBase
    {
        private readonly ISetService _setService;

        public SetsController(ISetService setService)
        {
            _setService = setService;
        }

        /// <summary>
        /// Update an existing exercise set
        /// </summary>
        /// <param name="setId">The unique identifier of the set to update</param>
        /// <param name="request">Set update data including reps, weight, and order</param>
        /// <returns>Updated set details</returns>
        /// <response code="200">Set updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Set not found or doesn't belong to current user</response>
        [HttpPatch("{setId}")]
        [ProducesResponseType(typeof(SetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Delete an exercise set
        /// </summary>
        /// <param name="setId">The unique identifier of the set to delete</param>
        /// <returns>No content on successful deletion</returns>
        /// <response code="204">Set deleted successfully</response>
        /// <response code="401">Unauthorized - invalid or missing access token</response>
        /// <response code="404">Set not found or doesn't belong to current user</response>
        [HttpDelete("{setId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        }
    }
}