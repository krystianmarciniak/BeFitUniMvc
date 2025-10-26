using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Data;
using BeFitUniMvc.Features.Exercises;

[Authorize]
public class PerformedExercisesController : Controller
{
    private readonly ApplicationDbContext _context;
    public PerformedExercisesController(ApplicationDbContext context) => _context = context;

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // jeden, spójny helper do SelectList
    private void PopulateSelectLists(int? trainingSessionId = null, int? exerciseTypeId = null)
    {
        var uid = GetUserId();

        var sessions = _context.TrainingSessions
            .Where(t => t.UserId == uid)
            .OrderByDescending(t => t.Date)
            .Select(t => new { t.Id, Text = t.Date.ToString("yyyy-MM-dd") + " • " + t.Title })
            .ToList();

        ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Text", trainingSessionId);

        var types = _context.ExerciseTypes
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name })
            .ToList();

        ViewData["ExerciseTypeId"] = new SelectList(types, "Id", "Name", exerciseTypeId);
    }

    // INDEX
    public async Task<IActionResult> Index()
    {
        var uid = GetUserId();
        var list = await _context.PerformedExercises
            .Include(p => p.TrainingSession)
            .Include(p => p.ExerciseType)
            .Where(p => p.TrainingSession.UserId == uid)
            .OrderByDescending(p => p.TrainingSession.Date)
            .AsNoTracking()
            .ToListAsync();

        return View(list);
    }

    // DETAILS
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var uid = GetUserId();
        var item = await _context.PerformedExercises
            .Include(p => p.TrainingSession)
            .Include(p => p.ExerciseType)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.TrainingSession.UserId == uid);

        if (item == null) return NotFound();
        return View(item);
    }

    // CREATE GET
    public IActionResult Create()
    {
        PopulateSelectLists();
        return View();
    }

    // CREATE POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TrainingSessionId,ExerciseTypeId,Sets,Reps,Weight,Notes")] PerformedExercise pe)
    {
        if (ModelState.IsValid)
        {
            _context.Add(pe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateSelectLists(pe.TrainingSessionId, pe.ExerciseTypeId);
        return View(pe);
    }

    // EDIT GET
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.PerformedExercises.FindAsync(id);
        if (item == null) return NotFound();

        PopulateSelectLists(item.TrainingSessionId, item.ExerciseTypeId);
        return View(item);
    }

    // EDIT POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TrainingSessionId,ExerciseTypeId,Sets,Reps,Weight,Notes")] PerformedExercise pe)
    {
        if (id != pe.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(pe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateSelectLists(pe.TrainingSessionId, pe.ExerciseTypeId);
        return View(pe);
    }

    // DELETE GET
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var uid = GetUserId();
        var item = await _context.PerformedExercises
            .Include(p => p.TrainingSession)
            .Include(p => p.ExerciseType)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.TrainingSession.UserId == uid);

        if (item == null) return NotFound();
        return View(item);
    }

    // DELETE POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var uid = GetUserId();
        var item = await _context.PerformedExercises
            .Include(p => p.TrainingSession)
            .FirstOrDefaultAsync(p => p.Id == id && p.TrainingSession.UserId == uid);

        if (item != null)
        {
            _context.PerformedExercises.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
