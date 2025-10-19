using System.ComponentModel.DataAnnotations;

namespace SimplyTrack.Api.Models.DTOs
{
    public class SetDto
    {
        public string Id { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public int Reps { get; set; }
        public double Weight { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSetRequestDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Reps must be at least 1")]
        public int Reps { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be non-negative")]
        public double Weight { get; set; }

        public string? ClientId { get; set; }
    }

    public class UpdateSetDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Reps must be at least 1")]
        public int? Reps { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Weight must be non-negative")]
        public double? Weight { get; set; }
    }
}