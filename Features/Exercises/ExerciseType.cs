using System.ComponentModel.DataAnnotations;

namespace BeFitUniMvc.Features.Exercises;

public class ExerciseType
{
    public int Id { get; set; }

    [Required, StringLength(64)]
    [Display(Name = "Nazwa ćwiczenia")]
    public string Name { get; set; } = string.Empty;

    [StringLength(256)]
    [Display(Name = "Opis ćwiczenia")]
    public string? Description { get; set; }

    public ICollection<PerformedExercise> PerformedExercises { get; set; } = new List<PerformedExercise>();
}
    