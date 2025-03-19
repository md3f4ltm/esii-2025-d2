using BlazorApp1.Client.Pages;
using BlazorApp1.Components;
using BlazorApp1.Data;
using BlazorApp1.Services;
using ApplicationUser = BlazorApp1.Models.ApplicationUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Register UserService
builder.Services.AddScoped<IUserService, UserService>();

// Register DatabaseService
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

var app = builder.Build();

using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

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
            if (testUser.Email != null)
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

        Console.WriteLine("\n--- Retrieving all users from database ---");
        var allUsers = await userManager.Users.ToListAsync();
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
    // Enable Swagger in development
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp1.Client._Imports).Assembly);

app.MapControllers();

app.Run();
