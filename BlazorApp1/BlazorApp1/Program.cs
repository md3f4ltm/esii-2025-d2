using ESII2025d2.Components; // Likely your App component location
using ESII2025d2.Models;     // Your Utilizador and ApplicationDbContext location
using Microsoft.AspNetCore.Components; // For NavigationManager
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- FROM CONFIG 1: Database Context ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- FROM CONFIG 1: Full Identity Setup ---
builder.Services.AddIdentity<Utilizador, IdentityRole>(options =>
    {
        // Configure Identity options here if needed
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>() // Use EF Core
    .AddDefaultTokenProviders(); // Standard tokens

// --- FROM CONFIG 1: Configure Application Cookie ---
// Use this INSTEAD of Config 2's AddAuthentication().AddCookie()
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/login"; // Path handled by Blazor component
    options.AccessDeniedPath = "/Account/AccessDenied"; // Or another Blazor page
    options.SlidingExpiration = true;

    // Keep these events if your Blazor app makes API calls and needs 401/403
    options.Events.OnRedirectToLogin = context => {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context => {
         context.Response.StatusCode = StatusCodes.Status403Forbidden;
         return Task.CompletedTask;
    };
});

// --- FROM CONFIG 1: Add Auth Services ---
// AddAuthentication is implicitly called by AddIdentity, but explicitly adding is fine.
// AddAuthorization is needed.
builder.Services.AddAuthentication(); // Ensure Authentication services are added
builder.Services.AddAuthorization(); // Ensure Authorization services are added


// --- FROM CONFIG 2: Blazor Web App Services ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // Or .AddInteractiveWebAssemblyComponents() if needed

// Add Controllers for your API endpoints
builder.Services.AddControllers();

// Add HttpClient (using NavigationManager for base URI is better than hardcoding)
// Use the HttpClient setup from Config 1
builder.Services.AddScoped(sp => {
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});


// --- Swagger Setup (from either config, this is fine) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "esii2025d2 API", Version = "v1" });
});

// Add Logging
builder.Services.AddLogging();


var app = builder.Build();

// --- Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // More detailed errors in dev
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "esii2025d2 API V1"));
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // Needed before Auth

// --- FROM CONFIG 1 & 2: Authentication/Authorization Middleware ---
// Order is crucial: Authentication then Authorization
app.UseAuthentication();
app.UseAuthorization();

// --- Antiforgery (Required for interactive components in Blazor Web App) ---
app.UseAntiforgery();

// Map API controllers
app.MapControllers();

// --- FROM CONFIG 2: Map Blazor Components ---
// Use this INSTEAD of MapFallbackToPage and MapBlazorHub
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // Match service registration


app.Run();
