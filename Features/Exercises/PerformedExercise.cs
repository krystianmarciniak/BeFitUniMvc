using System.ComponentModel.DataAnnotations;
using BeFitUniMvc.Features.Sessions;

namespace BeFitUniMvc.Features.Exercises
{
    public class PerformedExercise
    {
        public int Id { get; set; }

        [Display(Name = "Sesja treningowa")]
        public int TrainingSessionId { get; set; }

        [Display(Name = "Rodzaj ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        [Display(Name = "Serie"), Range(1, 50)]
        public int Sets { get; set; }

        [Display(Name = "Powtórzenia"), Range(1, 100)]
        public int Reps { get; set; }

        [Display(Name = "Ciężar (kg)"), Range(0, 1000)]
        public decimal? Weight { get; set; }

        [Display(Name = "Notatki")]
        public string? Notes { get; set; }

        [Display(Name = "Sesja")]
        public TrainingSession? TrainingSession { get; set; }

        [Display(Name = "Ćwiczenie")]
        public ExerciseType? ExerciseType { get; set; }
    }
}
