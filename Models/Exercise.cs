using System.ComponentModel.DataAnnotations;

namespace SimplyTrack.Api.Models
{
    public class Exercise
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        // User ownership - nullable for shared/global templates
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}