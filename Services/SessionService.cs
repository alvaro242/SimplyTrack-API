using ApplicationDBContext.Api.Data;
using Microsoft.EntityFrameworkCore;
using SimplyTrack.Api.Models;
using SimplyTrack.Api.Models.DTOs;

namespace SimplyTrack.Api.Services
{
    public interface ISessionService
    {
        Task<List<SessionDto>> GetExerciseSessionsAsync(string exerciseId, string userId, int limit = 50, int offset = 0, DateTime? from = null, DateTime? to = null);
        Task<SessionDetailDto?> GetSessionDetailAsync(string sessionId, string userId);
        Task<Session?> CreateSessionAsync(string exerciseId, CreateSessionRequestDto request, string userId);
        Task<bool> DeleteSessionAsync(string sessionId, string userId);
        Task UpdateSessionTotalsAsync(string sessionId);
    }

    public class SessionService : ISessionService
    {
        private readonly ApplicationDbContext _context;

        public SessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SessionDto>> GetExerciseSessionsAsync(string exerciseId, string userId, int limit = 50, int offset = 0, DateTime? from = null, DateTime? to = null)
        {
            // Verify user owns the exercise
            var exerciseExists = await _context.Exercises
                .AnyAsync(e => e.Id == exerciseId && e.UserId == userId);

            if (!exerciseExists)
                return new List<SessionDto>();

            var query = _context.Sessions
                .Where(s => s.ExerciseId == exerciseId);

            if (from.HasValue)
                query = query.Where(s => s.Date >= from.Value);

            if (to.HasValue)
                query = query.Where(s => s.Date <= to.Value);

            var sessions = await query
                .OrderByDescending(s => s.Date)
                .ThenByDescending(s => s.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return sessions.Select(s => new SessionDto
            {
                Id = s.Id,
                ExerciseId = s.ExerciseId,
                Date = s.Date,
                CreatedAt = s.CreatedAt,
                TotalWeight = s.TotalWeight,
                TotalReps = s.TotalReps,
                SetsCount = s.SetsCount
            }).ToList();
        }

        public async Task<SessionDetailDto?> GetSessionDetailAsync(string sessionId, string userId)
        {
            var session = await _context.Sessions
                .Include(s => s.Exercise)
                .Where(s => s.Id == sessionId && s.Exercise.UserId == userId)
                .FirstOrDefaultAsync();

            if (session == null)
                return null;

            var sets = await _context.Sets
                .Where(s => s.SessionId == sessionId)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();

            return new SessionDetailDto
            {
                Session = new SessionDto
                {
                    Id = session.Id,
                    ExerciseId = session.ExerciseId,
                    Date = session.Date,
                    CreatedAt = session.CreatedAt,
                    TotalWeight = session.TotalWeight,
                    TotalReps = session.TotalReps,
                    SetsCount = session.SetsCount
                },
                Sets = sets.Select(s => new SetDto
                {
                    Id = s.Id,
                    SessionId = s.SessionId,
                    Reps = s.Reps,
                    Weight = s.Weight,
                    CreatedAt = s.CreatedAt
                }).ToList()
            };
        }

        public async Task<Session?> CreateSessionAsync(string exerciseId, CreateSessionRequestDto request, string userId)
        {
            // Verify user owns the exercise
            var exerciseExists = await _context.Exercises
                .AnyAsync(e => e.Id == exerciseId && e.UserId == userId);

            if (!exerciseExists)
                return null;

            var session = new Session
            {
                ExerciseId = exerciseId,
                Date = request.Date ?? DateTime.Today,
                CreatedAt = DateTime.UtcNow,
                TotalWeight = 0,
                TotalReps = 0,
                SetsCount = 0
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<bool> DeleteSessionAsync(string sessionId, string userId)
        {
            var session = await _context.Sessions
                .Include(s => s.Exercise)
                .Where(s => s.Id == sessionId && s.Exercise.UserId == userId)
                .FirstOrDefaultAsync();

            if (session == null)
                return false;

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateSessionTotalsAsync(string sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            if (session == null)
                return;

            var sets = await _context.Sets
                .Where(s => s.SessionId == sessionId)
                .ToListAsync();

            session.SetsCount = sets.Count;
            session.TotalReps = sets.Sum(s => s.Reps);
            session.TotalWeight = sets.Sum(s => s.Weight * s.Reps);

            await _context.SaveChangesAsync();
        }
    }
}