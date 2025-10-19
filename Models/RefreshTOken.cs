using System.ComponentModel.DataAnnotations;

namespace SimplyTrack.Api.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public string Token { get; set; }
        public string UserId { get; set; }

        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }

        public bool IsActive => Revoked == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expires;
    }
}