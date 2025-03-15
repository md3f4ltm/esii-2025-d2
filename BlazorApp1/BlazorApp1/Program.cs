using BlazorApp1.Client.Pages;
using BlazorApp1.Components;
using BlazorApp1.Data;
// Use fully qualified name to avoid conflicts
using ApplicationUser = BlazorApp1.Models.ApplicationUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add this after AddRazorComponents() section
builder.Services.AddControllers();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add database context and identity services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Update the user creation section to add more logging
        Console.WriteLine("Attempting to seed test users...");

        var testUsers = new List<ApplicationUser>
        {
            new ApplicationUser { UserName = "testuser1", Email = "test1@example.com", EmailConfirmed = true, FirstName = "Test1", LastName = "User1" },
            new ApplicationUser { UserName = "testuser2", Email = "test2@example.com", EmailConfirmed = true, FirstName = "Test2", LastName = "User2" },
            new ApplicationUser { UserName = "testuser3", Email = "test3@example.com", EmailConfirmed = true, FirstName = "Test3", LastName = "User3" },
            new ApplicationUser { UserName = "testuser4", Email = "test4@example.com", EmailConfirmed = true, FirstName = "Test4", LastName = "User4" }
        };

        foreach (var testUser in testUsers)
        {
            if (testUser.Email != null) // Fix null reference warning
            {
                var existingUser = await userManager.FindByEmailAsync(testUser.Email);
                Console.WriteLine($"Existing user check for {testUser.Email}: {(existingUser == null ? "Not found" : "Found")}");

                if (existingUser == null)
                {
                    Console.WriteLine($"Creating test user {testUser.Email}...");
                    var result = await userManager.CreateAsync(testUser, "Test123!");

                    if (result.Succeeded)
                    {
                        Console.WriteLine($"Test user {testUser.Email} created successfully!");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create test user {testUser.Email}: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            else
            {
                Console.WriteLine("Skipping user with null email");
            }
        }

        // Add this section to retrieve and print all users after seeding
        Console.WriteLine("\n--- Retrieving all users from database ---");
        var allUsers = userManager.Users.ToList();
        Console.WriteLine($"Found {allUsers.Count} users in the database:");

        foreach (var user in allUsers)
        {
            Console.WriteLine($"User: {user.UserName}, Email: {user.Email}, Name: {user.FirstName} {user.LastName}");
        }
        Console.WriteLine("--- End of user list ---\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error seeding database: " + ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Use CORS before routing
app.UseCors("AllowAll");

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp1.Client._Imports).Assembly);

// Add this before app.Run()
app.MapControllers();

app.Run();