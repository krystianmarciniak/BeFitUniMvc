using System.ComponentModel.DataAnnotations;
using BeFitUniMvc.Features.Exercises;

namespace BeFitUniMvc.Features.Sessions;

public class TrainingSession
{
    public int Id { get; set; }

    [Required, StringLength(64)]
    [Display(Name = "Tytuł sesji treningowej")]
    public string Title { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Data")]
    public DateTime Date { get; set; } = DateTime.Today;

    // właściciel rekordu
    [Display(Name = "Użytkownik")]
    public string UserId { get; set; } = string.Empty;

    public ICollection<PerformedExercise> PerformedExercises { get; set; } = new List<PerformedExercise>();
}
