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

  [HttpGet]
public async Task<IActionResult> Edit(int? id)
{
    if (id is null) return NotFound();
    var uid = _userManager.GetUserId(User);

    var entity = await _context.TrainingSessions
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);

    if (entity == null) return NotFound();
    return View(entity);
}


    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, TrainingSession input)
{
    if (id != input.Id) return NotFound();

    // Walidacja domenowa: koniec po początku
    if (input.EndTime <= input.StartTime)
        ModelState.AddModelError(nameof(input.EndTime), "Koniec musi być po początku.");

    if (!ModelState.IsValid)
        return View(input);

    var uid = _userManager.GetUserId(User);

    // Pobierz encję zalogowanego użytkownika (bez AsNoTracking – chcemy śledzenia zmian)
    var entity = await _context.TrainingSessions
        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);

    // Jeśli brak – nie istnieje lub nie należy do użytkownika
    if (entity == null) return NotFound();

    // Aktualizacja tylko dozwolonych pól
    entity.Title = input.Title;
    entity.StartTime = input.StartTime;
    entity.EndTime = input.EndTime;

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
