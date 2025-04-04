using ESII2025d2.Components; // Likely your App component location
using ESII2025d2.Models;    // Your Utilizador and ApplicationDbContext location
using Microsoft.AspNetCore.Components; // For NavigationManager
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Database Context ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- Identity Setup ---
builder.Services.AddIdentity<Utilizador, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// --- Application Cookie Configuration ---
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.SameSite = SameSiteMode.Lax; 

   });

// --- Auth Services ---
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// *** Antiforgery Services ***
builder.Services.AddAntiforgery();

// --- Blazor Web App Services ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Controllers for your API endpoints
// REMOVED .ConfigureApiBehaviorOptions - Not needed for current controller logic
builder.Services.AddControllersWithViews();

// Add HttpClient
builder.Services.AddScoped(sp => {
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    var baseAddress = navigationManager.BaseUri.EndsWith('/') ? navigationManager.BaseUri : navigationManager.BaseUri + "/";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});

// --- Swagger Setup ---
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
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "esii2025d2 API V1"));
    // app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // Needed before Auth

// Authentication/Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Antiforgery Middleware
app.UseAntiforgery();

// Map API controllers
app.MapControllers();

// Map Blazor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
