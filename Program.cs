using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// DbContext z SQL Server (LocalDB)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Podgląd wyjątków EF na DEV (opcjonalnie, ale wygodne na laby)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity + RoleManager. Wymagane przez wymagania (Admin ogranicza CRUD typów ćwiczeń)
builder.Services.AddDefaultIdentity<IdentityUser>(o => o.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Dodanie wsparcia dla kontrolerów z widokami. Minimalne MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// (Opcjonalnie) kultura PL
var culture = new CultureInfo("pl-PL");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// SEED ról i konta admina, w zadaniu potrzebna jest rola Admin.
// zmiana hasła po pierwszym logowaniu.
using (var scope = app.Services.CreateScope())
{
  var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
  var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

  foreach (var r in new[] { "Admin", "User" })
    if (!await roleMgr.RoleExistsAsync(r))
      await roleMgr.CreateAsync(new IdentityRole(r));

  var adminEmail = "admin@befit.uni";
  var admin = await userMgr.FindByEmailAsync(adminEmail);
  if (admin is null)  {
    admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
    await userMgr.CreateAsync(admin, "ZmienMnie!123"); // pierwsze hasło do zmiany
    await userMgr.AddToRoleAsync(admin, "Admin");
  }
}

if (app.Environment.IsDevelopment()){
  app.UseMigrationsEndPoint();
}else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // bo istnieje Identity
app.UseAuthorization();

// Trasa MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages (Identity UI)
app.MapRazorPages();

app.Run();
