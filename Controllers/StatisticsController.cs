using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Data;
using BeFitUniMvc.Models;

[Authorize]
public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public StatisticsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var uid = _userManager.GetUserId(User);

        var q = _context.PerformedExercises
            .Include(pe => pe.ExerciseType)
            .Include(pe => pe.TrainingSession)
            .Where(pe => pe.TrainingSession != null &&
                         pe.TrainingSession.UserId == uid)
            .AsQueryable();

        if (from.HasValue)
            q = q.Where(pe => pe.TrainingSession!.StartTime >= from.Value);

        if (to.HasValue)
            q = q.Where(pe => pe.TrainingSession!.EndTime <= to.Value);

        var totalExercises = await q.CountAsync();
        var totalVolume = await q.SumAsync(pe => (decimal)(pe.Weight ?? 0m) * pe.Reps);

        var top = await q.GroupBy(pe => pe.ExerciseType!.Name)
            .Select(g => new TopExerciseVm
            {
                Exercise = g.Key,
                Sessions = g.Select(x => x.TrainingSessionId).Distinct().Count(),
                Sets = g.Sum(x => x.Sets),
                Reps = g.Sum(x => x.Reps),
                Volume = g.Sum(x => (decimal)(x.Weight ?? 0m) * x.Reps)
            })
            .OrderByDescending(x => x.Volume)
            .Take(10)
            .ToListAsync();

        var vm = new StatsVm
        {
            From = from,
            To = to,
            TotalExercises = totalExercises,
            TotalVolume = totalVolume,
            TopExercises = top
        };

        return View(vm);
    }
}
