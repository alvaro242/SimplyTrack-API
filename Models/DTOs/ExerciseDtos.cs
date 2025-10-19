using System.ComponentModel.DataAnnotations;

namespace SimplyTrack.Api.Models.DTOs
{
    public class ExerciseDto
    {
        public string Id { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateExerciseRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateExerciseDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class ExerciseDashboardItemDto
    {
        public ExerciseDto Exercise { get; set; } = new();
        public LastSessionDto? LastSession { get; set; }
    }

    public class LastSessionDto
    {
        public string SessionId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double TotalWeight { get; set; }
        public int TotalReps { get; set; }
        public int SetsCount { get; set; }
    }

    public class ExerciseDetailDto
    {
        public ExerciseDto Exercise { get; set; } = new();
        public int SessionsCount { get; set; }
        public SessionDto? LastSession { get; set; }
    }
}