using ApplicationDBContext.Api.Data;
using Microsoft.EntityFrameworkCore;
using SimplyTrack.Api.Models;
using SimplyTrack.Api.Models.DTOs;

namespace SimplyTrack.Api.Services
{
    public interface ISetService
    {
        Task<SetEntity?> CreateSetAsync(string sessionId, CreateSetRequestDto request, string userId);
        Task<SetEntity?> UpdateSetAsync(string setId, UpdateSetDto request, string userId);
        Task<bool> DeleteSetAsync(string setId, string userId);
    }

    public class SetService : ISetService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISessionService _sessionService;

        public SetService(ApplicationDbContext context, ISessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        public async Task<SetEntity?> CreateSetAsync(string sessionId, CreateSetRequestDto request, string userId)
        {
            // Verify user owns the session through exercise
            var sessionExists = await _context.Sessions
                .Include(s => s.Exercise)
                .AnyAsync(s => s.Id == sessionId && s.Exercise.UserId == userId);

            if (!sessionExists)
                return null;

            var set = new SetEntity
            {
                SessionId = sessionId,
                Reps = request.Reps,
                Weight = request.Weight,
                CreatedAt = DateTime.UtcNow
            };

            _context.Sets.Add(set);
            await _context.SaveChangesAsync();

            // Update session totals
            await _sessionService.UpdateSessionTotalsAsync(sessionId);

            return set;
        }

        public async Task<SetEntity?> UpdateSetAsync(string setId, UpdateSetDto request, string userId)
        {
            var set = await _context.Sets
                .Include(s => s.Session)
                .ThenInclude(s => s.Exercise)
                .Where(s => s.Id == setId && s.Session.Exercise.UserId == userId)
                .FirstOrDefaultAsync();

            if (set == null)
                return null;

            if (request.Reps.HasValue)
                set.Reps = request.Reps.Value;

            if (request.Weight.HasValue)
                set.Weight = request.Weight.Value;

            await _context.SaveChangesAsync();

            // Update session totals
            await _sessionService.UpdateSessionTotalsAsync(set.SessionId);

            return set;
        }

        public async Task<bool> DeleteSetAsync(string setId, string userId)
        {
            var set = await _context.Sets
                .Include(s => s.Session)
                .ThenInclude(s => s.Exercise)
                .Where(s => s.Id == setId && s.Session.Exercise.UserId == userId)
                .FirstOrDefaultAsync();

            if (set == null)
                return false;

            var sessionId = set.SessionId;
            _context.Sets.Remove(set);
            await _context.SaveChangesAsync();

            // Update session totals
            await _sessionService.UpdateSessionTotalsAsync(sessionId);

            return true;
        }
    }
}