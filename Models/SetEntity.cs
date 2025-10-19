using System.ComponentModel.DataAnnotations;

namespace SimplyTrack.Api.Models
{
    public class SetEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SessionId { get; set; }
        public Session Session { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}