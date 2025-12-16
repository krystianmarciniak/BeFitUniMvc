using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Data;
using BeFitUniMvc.Features.Exercises;



[Authorize(Roles = "Admin")]
public class ExerciseTypesController : Controller
{
    private readonly ApplicationDbContext _context;
    public ExerciseTypesController(ApplicationDbContext context) => _context = context;

    [AllowAnonymous]
    public async Task<IActionResult> Index()
        => View(await _context.ExerciseTypes.AsNoTracking().ToListAsync());

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var exerciseType = await _context.ExerciseTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (exerciseType == null) return NotFound();
        return View(exerciseType);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] ExerciseType model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var et = await _context.ExerciseTypes.FindAsync(id);
        if (et == null) return NotFound();
        return View(et);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ExerciseType model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);

        try
        {
            _context.Update(model);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.ExerciseTypes.AnyAsync(e => e.Id == id))
                return NotFound();
            throw;
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var et = await _context.ExerciseTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (et == null) return NotFound();
        return View(et);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await _context.ExerciseTypes.FindAsync(id);
    if (entity == null) return NotFound();

    try
    {
        _context.ExerciseTypes.Remove(entity);
        await _context.SaveChangesAsync();
        TempData["Msg"] = "Usunięto rodzaj ćwiczenia.";
    }
    catch (DbUpdateException)
    {
        // naruszenie FK – typ użyty w PerformedExercises
        TempData["Error"] = "Nie można usunąć: ten rodzaj ćwiczenia jest użyty w wykonanych ćwiczeniach.";
    }

    return RedirectToAction(nameof(Index)); 
    }
}
