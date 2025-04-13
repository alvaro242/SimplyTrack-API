using System;
using System.Collections.Generic;

namespace SimplyTrack_API.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        public int WorkoutId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Reps { get; set; }

        public int Sets { get; set; }

        public double Weight { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Workout? Workout { get; set; }

        public ICollection<WeightProgression> WeightProgressions { get; set; } = new List<WeightProgression>();
    }
}
