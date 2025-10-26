using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Data;

[Authorize]
public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;
    public StatisticsController(ApplicationDbContext context) => _context = context;
    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var uid = GetUserId();

        var q = _context.PerformedExercises
            .Include(p => p.TrainingSession)
            .Include(p => p.ExerciseType)
            .Where(p => p.TrainingSession.UserId == uid);

        if (from.HasValue) q = q.Where(p => p.TrainingSession.Date >= from.Value);
        if (to.HasValue) q = q.Where(p => p.TrainingSession.Date <= to.Value);

        var data = await q.ToListAsync();

        var vm = new StatsVm
        {
            From = from,
            To = to,
            TotalExercises = data.Count,
            TotalSets = data.Sum(p => p.Sets),
            TotalReps = data.Sum(p => p.Reps),
            TotalVolume = data.Sum(p => (p.Weight ?? 0) * p.Reps * p.Sets),
            TopExercises = data
                .GroupBy(p => p.ExerciseType?.Name ?? "(brak)")
                .Select(g => new TopExerciseVm
                {
                    Exercise = g.Key,
                    Sessions = g.Select(x => x.TrainingSessionId).Distinct().Count(),
                    Sets = g.Sum(x => x.Sets),
                    Reps = g.Sum(x => x.Reps),
                    Volume = g.Sum(x => (x.Weight ?? 0) * x.Reps * x.Sets)
                })
                .OrderByDescending(t => t.Volume)
                .Take(5)
                .ToList()
        };

        return View(vm);
    }
}

// --- Modele widoku (ViewModels) ---
public class StatsVm
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int TotalExercises { get; set; }
    public int TotalSets { get; set; }
    public int TotalReps { get; set; }
    public decimal TotalVolume { get; set; }
    public List<TopExerciseVm> TopExercises { get; set; } = new();
}

public class TopExerciseVm
{
    public string Exercise { get; set; } = "";
    public int Sessions { get; set; }
    public int Sets { get; set; }
    public int Reps { get; set; }
    public decimal Volume { get; set; }
}
