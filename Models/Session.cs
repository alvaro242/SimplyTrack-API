using System.ComponentModel.DataAnnotations;

namespace SimplyTrack.Api.Models
{
    public class Session
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ExerciseId { get; set; } = string.Empty;
        public Exercise? Exercise { get; set; }

        // store date as DateTime (date-only semantics)
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // materialized/calculated fields (can be computed on demand)
        public double TotalWeight { get; set; }
        public int TotalReps { get; set; }
        public int SetsCount { get; set; }

        public ICollection<SetEntity> Sets { get; set; }
    }
}