using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Data;
using BeFitUniMvc.Features.Sessions;

[Authorize]
public class TrainingSessionsController : Controller
{
    private readonly ApplicationDbContext _context;
    public TrainingSessionsController(ApplicationDbContext context) => _context = context;
    private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<IActionResult> Index()
    {
        var uid = GetUserId();
        var list = await _context.TrainingSessions
            .Where(t => t.UserId == uid)
            .OrderByDescending(t => t.Date)
            .AsNoTracking()
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var uid = GetUserId();
        var item = await _context.TrainingSessions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == uid);
        if (item == null) return NotFound();
        return View(item);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Date")] TrainingSession model)
    {
        if (!ModelState.IsValid) return View(model);
        model.UserId = GetUserId();
        _context.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var uid = GetUserId();
        var item = await _context.TrainingSessions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == uid);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date")] TrainingSession model)
    {
        if (id != model.Id) return NotFound();
        var uid = GetUserId();

        var original = await _context.TrainingSessions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == uid);
        if (original == null) return NotFound();
        if (!ModelState.IsValid) return View(original);

        original.Title = model.Title;
        original.Date  = model.Date;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var uid = GetUserId();
        var item = await _context.TrainingSessions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == uid);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var uid = GetUserId();
        var item = await _context.TrainingSessions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == uid);
        if (item != null)
        {
            _context.TrainingSessions.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
