using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Data;
using BeFitUniMvc.Models;


[Authorize] // tylko zalogowani
public class TrainingSessionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public TrainingSessionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var uid = _userManager.GetUserId(User);
        var list = await _context.TrainingSessions
            .Where(x => x.UserId == uid)
            .AsNoTracking()
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var uid = _userManager.GetUserId(User);

        var item = await _context.TrainingSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);

        return item == null ? Forbid() : View(item);
    }

    public IActionResult Create() => View();

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Title,StartTime,EndTime")] TrainingSession session)
{
    if (session.EndTime <= session.StartTime)
        ModelState.AddModelError(nameof(TrainingSession.EndTime), "Koniec musi być po początku.");

    if (!ModelState.IsValid)
        return View(session);

    // przypisanie do zalogowanego użytkownika
    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    session.UserId = userId;

    _context.TrainingSessions.Add(session);
    await _context.SaveChangesAsync();

    TempData["Msg"] = "Sesja zapisana.";
    return RedirectToAction(nameof(Index));
}


    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var uid = _userManager.GetUserId(User);

        var item = await _context.TrainingSessions.FindAsync(id);
        if (item == null) return NotFound();
        if (item.UserId != uid) return Forbid();

        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TrainingSession model)
    {
        if (id != model.Id) return NotFound();

        if (model.EndTime < model.StartTime)
            ModelState.AddModelError(nameof(model.EndTime), "Koniec nie może być wcześniejszy niż początek.");
        if (!ModelState.IsValid) return View(model);

        var uid = _userManager.GetUserId(User);
        var original = await _context.TrainingSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (original == null) return NotFound();
        if (original.UserId != uid) return Forbid();

        model.UserId = uid; // utrzymujemy właściciela
        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var uid = _userManager.GetUserId(User);

        var item = await _context.TrainingSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);

        return item == null ? Forbid() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var uid = _userManager.GetUserId(User);
        var item = await _context.TrainingSessions.FindAsync(id);
        if (item == null) return NotFound();
        if (item.UserId != uid) return Forbid();

        _context.TrainingSessions.Remove(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
