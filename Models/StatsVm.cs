namespace BeFitUniMvc.Models;

public class StatsVm
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int TotalExercises { get; set; }
    public int TotalSets { get; set; }
    public int TotalReps { get; set; }
    public decimal TotalVolume { get; set; }

    // Tabela Top ćwiczeń
    public List<TopExerciseVm> TopExercises { get; set; } = new();
}

public class TopExerciseVm
{
    public string Exercise { get; set; } = string.Empty;
    public int Sessions { get; set; }

    public int Sets { get; set; }
    public int Reps { get; set; }
    public decimal Volume { get; set; } // suma (Weight*Reps)
}
