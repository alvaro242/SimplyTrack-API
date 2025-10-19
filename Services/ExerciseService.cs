using ApplicationDBContext.Api.Data;
using Microsoft.EntityFrameworkCore;
using SimplyTrack.Api.Models;
using SimplyTrack.Api.Models.DTOs;

namespace SimplyTrack.Api.Services
{
    public interface IExerciseService
    {
        Task<List<ExerciseDashboardItemDto>> GetUserExercisesAsync(string userId, string? searchQuery = null, bool includeLastSession = true);
        Task<ExerciseDetailDto?> GetExerciseDetailAsync(string exerciseId, string userId);
        Task<Exercise> CreateExerciseAsync(CreateExerciseRequestDto request, string userId);
        Task<Exercise?> UpdateExerciseAsync(string exerciseId, UpdateExerciseDto request, string userId);
        Task<bool> DeleteExerciseAsync(string exerciseId, string userId);
    }

    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext _context;

        public ExerciseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExerciseDashboardItemDto>> GetUserExercisesAsync(string userId, string? searchQuery = null, bool includeLastSession = true)
        {
            var query = _context.Exercises
                .Where(e => e.UserId == userId);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(e => e.Name.Contains(searchQuery));
            }

            var exercises = await query
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            var result = new List<ExerciseDashboardItemDto>();

            foreach (var exercise in exercises)
            {
                var item = new ExerciseDashboardItemDto
                {
                    Exercise = new ExerciseDto
                    {
                        Id = exercise.Id,
                        UserId = exercise.UserId,
                        Name = exercise.Name,
                        Notes = exercise.Description,
                        CreatedAt = exercise.CreatedAt
                    }
                };

                if (includeLastSession)
                {
                    var lastSession = await _context.Sessions
                        .Where(s => s.ExerciseId == exercise.Id)
                        .OrderByDescending(s => s.Date)
                        .ThenByDescending(s => s.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (lastSession != null)
                    {
                        item.LastSession = new LastSessionDto
                        {
                            SessionId = lastSession.Id,
                            Date = lastSession.Date,
                            TotalWeight = lastSession.TotalWeight,
                            TotalReps = lastSession.TotalReps,
                            SetsCount = lastSession.SetsCount
                        };
                    }
                }

                result.Add(item);
            }

            return result;
        }

        public async Task<ExerciseDetailDto?> GetExerciseDetailAsync(string exerciseId, string userId)
        {
            var exercise = await _context.Exercises
                .Where(e => e.Id == exerciseId && e.UserId == userId)
                .FirstOrDefaultAsync();

            if (exercise == null)
                return null;

            var sessionsCount = await _context.Sessions
                .CountAsync(s => s.ExerciseId == exerciseId);

            var lastSession = await _context.Sessions
                .Where(s => s.ExerciseId == exerciseId)
                .OrderByDescending(s => s.Date)
                .ThenByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            return new ExerciseDetailDto
            {
                Exercise = new ExerciseDto
                {
                    Id = exercise.Id,
                    UserId = exercise.UserId,
                    Name = exercise.Name,
                    Notes = exercise.Description,
                    CreatedAt = exercise.CreatedAt
                },
                SessionsCount = sessionsCount,
                LastSession = lastSession != null ? new SessionDto
                {
                    Id = lastSession.Id,
                    ExerciseId = lastSession.ExerciseId,
                    Date = lastSession.Date,
                    CreatedAt = lastSession.CreatedAt,
                    TotalWeight = lastSession.TotalWeight,
                    TotalReps = lastSession.TotalReps,
                    SetsCount = lastSession.SetsCount
                } : null
            };
        }

        public async Task<Exercise> CreateExerciseAsync(CreateExerciseRequestDto request, string userId)
        {
            var exercise = new Exercise
            {
                UserId = userId,
                Name = request.Name,
                Description = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            return exercise;
        }

        public async Task<Exercise?> UpdateExerciseAsync(string exerciseId, UpdateExerciseDto request, string userId)
        {
            var exercise = await _context.Exercises
                .Where(e => e.Id == exerciseId && e.UserId == userId)
                .FirstOrDefaultAsync();

            if (exercise == null)
                return null;

            if (!string.IsNullOrEmpty(request.Name))
                exercise.Name = request.Name;

            if (request.Notes != null)
                exercise.Description = request.Notes;

            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task<bool> DeleteExerciseAsync(string exerciseId, string userId)
        {
            var exercise = await _context.Exercises
                .Where(e => e.Id == exerciseId && e.UserId == userId)
                .FirstOrDefaultAsync();

            if (exercise == null)
                return false;

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}