using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;
using AdvancedProgrammingASPProject.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the main app DB context
builder.Services.AddDbContext<ProjectDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectDBContext")));

// Register the Identity DB context
builder.Services.AddDbContext<AdvancedProgrammingASPProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdvancedProgrammingASPProjectContextConnection")));

// Add Identity support
builder.Services.AddDefaultIdentity<Users>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AdvancedProgrammingASPProjectContext>();

builder.Services.AddScoped<LogService>();


var app = builder.Build();

//  SYNC USERS FROM MAIN DB TO IDENTITY DB
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedUsers.SyncMainUsersAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
