using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Data;
using BeFitUniMvc.Features.Exercises;

[Authorize] // tylko zalogowani
public class PerformedExercisesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public PerformedExercisesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var uid = _userManager.GetUserId(User);
        var items = await _context.PerformedExercises
            .Include(p => p.TrainingSession)
            .Include(p => p.ExerciseType)
            .Where(p => p.UserId == uid) // własne wpisy
            .AsNoTracking()
            .ToListAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var uid = _userManager.GetUserId(User);

        var item = await _context.PerformedExercises
            .Include(p => p.TrainingSession)
            .Include(p => p.ExerciseType)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == uid);

        return item == null ? Forbid() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        var uid = _userManager.GetUserId(User);
        ViewData["TrainingSessionId"] = new SelectList(
            await _context.TrainingSessions.Where(ts => ts.UserId == uid).ToListAsync(),
            "Id", "Title"
        );
        ViewData["ExerciseTypeId"] = new SelectList(await _context.ExerciseTypes.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TrainingSessionId,ExerciseTypeId,Sets,Reps,Weight,Notes")] PerformedExercise model)
    {
        var uid = _userManager.GetUserId(User);

        // bezpieczeństwo: sesja musi być własnością użytkownika
        var ownsSession = await _context.TrainingSessions.AnyAsync(ts => ts.Id == model.TrainingSessionId && ts.UserId == uid);
        if (!ownsSession)
        {
            ModelState.AddModelError(nameof(model.TrainingSessionId), "Nieprawidłowa sesja.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions.Where(ts => ts.UserId == uid), "Id", "Title", model.TrainingSessionId);
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", model.ExerciseTypeId);
            return View(model);
        }

        model.UserId = uid;
        _context.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var uid = _userManager.GetUserId(User);

        var item = await _context.PerformedExercises.FindAsync(id);
        if (item == null) return NotFound();
        if (item.UserId != uid) return Forbid();

        ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions.Where(ts => ts.UserId == uid), "Id", "Title", item.TrainingSessionId);
        ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", item.ExerciseTypeId);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TrainingSessionId,ExerciseTypeId,Sets,Reps,Weight,Notes")] PerformedExercise model)
    {
        if (id != model.Id) return NotFound();
        var uid = _userManager.GetUserId(User);

        var original = await _context.PerformedExercises.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (original == null) return NotFound();
        if (original.UserId != uid) return Forbid();

        // sesja nadal musi należeć do użytkownika
        var ownsSession = await _context.TrainingSessions.AnyAsync(ts => ts.Id == model.TrainingSessionId && ts.UserId == uid);
        if (!ownsSession)
            ModelState.AddModelError(nameof(model.TrainingSessionId), "Nieprawidłowa sesja.");

        if (!ModelState.IsValid)
        {
            ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions.Where(ts => ts.UserId == uid), "Id", "Title", model.TrainingSessionId);
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", model.ExerciseTypeId);
            return View(model);
        }

        model.UserId = uid; // utrzymujemy właściciela
        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var uid = _userManager.GetUserId(User);

        var item = await _context.PerformedExercises
            .Include(p => p.TrainingSession)
            .Include(p => p.ExerciseType)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == uid);

        return item == null ? Forbid() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var uid = _userManager.GetUserId(User);
        var item = await _context.PerformedExercises.FindAsync(id);
        if (item == null) return NotFound();
        if (item.UserId != uid) return Forbid();

        _context.PerformedExercises.Remove(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
