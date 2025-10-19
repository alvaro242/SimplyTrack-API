using System.ComponentModel.DataAnnotations;

namespace SimplyTrack.Api.Models.DTOs
{
    public class SessionDto
    {
        public string Id { get; set; } = string.Empty;
        public string ExerciseId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public double TotalWeight { get; set; }
        public int TotalReps { get; set; }
        public int SetsCount { get; set; }
    }

    public class CreateSessionRequestDto
    {
        public DateTime? Date { get; set; }
    }

    public class SessionDetailDto
    {
        public SessionDto Session { get; set; } = new();
        public List<SetDto> Sets { get; set; } = new();
    }

    public class SessionWithSetsDto
    {
        public SessionDto Session { get; set; } = new();
        public List<SetDto> Sets { get; set; } = new();
    }
}