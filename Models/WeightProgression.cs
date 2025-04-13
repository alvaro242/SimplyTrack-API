using System;

namespace SimplyTrack_API.Models
{
    public class WeightProgression
    {
        public int Id { get; set; }

        public int ExerciseId { get; set; }

        public double Weight { get; set; }

        public DateTime Date { get; set; }

        // Navigation
        public Exercise? Exercise { get; set; }
    }
}
