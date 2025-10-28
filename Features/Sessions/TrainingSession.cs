using System.ComponentModel.DataAnnotations;
using BeFitUniMvc.Features.Exercises;

namespace BeFitUniMvc.Models;

public class TrainingSession
{
    public int Id { get; set; }

    [Required, StringLength(64)]
    [Display(Name = "Tytuł sesji treningowej")]
    public string Title { get; set; } = string.Empty;

    [Required, DataType(DataType.DateTime)]
    [Display(Name = "Początek")]
    public DateTime StartTime { get; set; } = DateTime.Now;

    [Required, DataType(DataType.DateTime)]
    [Display(Name = "Koniec")]
    public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);

    // przypisywane automatycznie w kontrolerze; nie pokazujemy w formularzach
    [Display(Name = "Użytkownik")]
    public string? UserId { get; set; }

    public ICollection<PerformedExercise>? PerformedExercises { get; set; }
}
