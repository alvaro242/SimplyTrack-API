using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SimplyTrack_API.Models
{
    public class User : IdentityUser // Inherit from IdentityUser for built-in authentication features
    {
        

        [Required]
        public string Name { get; set; } = string.Empty;

        //I believe this is not needed as IdentityUser already has an Email property
      //  [Required, EmailAddress]
      //  public new string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    }
}
