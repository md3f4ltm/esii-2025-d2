using Microsoft.AspNetCore.Components.Authorization;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Microsoft.OpenApi.Models;

using esii_2025_d2.Client.Pages;

using esii_2025_d2.Components;

using esii_2025_d2.Components.Account;

using esii_2025_d2.Data;

using static esii_2025_d2.Data.SeedData;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;



var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddRazorComponents()

  .AddInteractiveServerComponents()

  .AddInteractiveWebAssemblyComponents()

  .AddAuthenticationStateSerialization();



// --->>> ADD CONTROLLER SERVICES <<<---

builder.Services.AddControllers(); // Registers services for API Controllers



builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityUserAccessor>();

builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Add HTTP context accessor for cookie authentication
builder.Services.AddHttpContextAccessor();

// Configure HttpClient for authenticated API calls
builder.Services.AddScoped<HttpClient>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5112/") };
    return httpClient;
});





builder.Services.AddAuthentication(options =>

  {

      options.DefaultScheme = IdentityConstants.ApplicationScheme;

      options.DefaultSignInScheme = IdentityConstants.ExternalScheme;

  })

  .AddIdentityCookies();



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>

  options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();



// --->>> ADD ROLE SERVICES <<<---

// Certifique-se de adicionar suporte a Roles ao Identity Core

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)

  .AddRoles<IdentityRole>() // <--- ADICIONE ISTO PARA SUPORTE A ROLES

    .AddEntityFrameworkStores<ApplicationDbContext>()

  .AddSignInManager()

  .AddDefaultTokenProviders();



builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Register application services
builder.Services.AddScoped<esii_2025_d2.Services.ICustomerService, esii_2025_d2.Services.CustomerService>();
builder.Services.AddScoped<esii_2025_d2.Services.ITalentService, esii_2025_d2.Services.TalentService>();
builder.Services.AddScoped<esii_2025_d2.Services.IExperienceService, esii_2025_d2.Services.ExperienceService>();
builder.Services.AddScoped<esii_2025_d2.Services.IJobProposalService, esii_2025_d2.Services.JobProposalService>();
builder.Services.AddScoped<esii_2025_d2.Services.IReportsService, esii_2025_d2.Services.ReportsService>();
builder.Services.AddScoped<esii_2025_d2.Services.ISkillService, esii_2025_d2.Services.SkillService>();
builder.Services.AddScoped<esii_2025_d2.Services.ITalentCategoryService, esii_2025_d2.Services.TalentCategoryService>();
builder.Services.AddScoped<esii_2025_d2.Services.ITalentSkillService, esii_2025_d2.Services.TalentSkillService>();
builder.Services.AddScoped<esii_2025_d2.Services.ICurrentUserTalentService, esii_2025_d2.Services.CurrentUserTalentService>();



// Swagger Service Configuration

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>

{

    c.SwaggerDoc("v1", new OpenApiInfo

    {

        Title = "esii_2025_d2 API",

        Version = "v1",

        Description = "API documentation for the esii_2025_d2 application"

    });

});

builder.Services.AddScoped<esii_2025_d2.Services.ICustomerService, esii_2025_d2.Services.CustomerService>();

// --- Construção da Aplicação ---

var app = builder.Build();



// --->>> CÓDIGO PARA CRIAR ROLES <<<---

// Coloque este bloco DEPOIS de 'var app = builder.Build();'

using (var scope = app.Services.CreateScope())

{

    var services = scope.ServiceProvider;

    try

    {

        // Pega no RoleManager a partir dos serviços configurados

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();



        // Lista das roles que quer garantir que existem

        string[] roleNames = { "Admin", "Talent", "Customer" };

        IdentityResult roleResult;



        foreach (var roleName in roleNames)

        {

            // Verifica se a role já existe na base de dados

            var roleExist = await roleManager.RoleExistsAsync(roleName);

            if (!roleExist)

            {

                // Se não existir, cria a role

                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

                // (Opcional: pode adicionar logging aqui para saber se a role foi criada)

                // if (roleResult.Succeeded) { Console.WriteLine($"Role '{roleName}' created successfully."); }

                // else { /* Log errors */ }

            }

        }

    }

    catch (Exception ex)

    {

        // (Opcional: Adicionar logging para capturar erros durante a criação de roles)

        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while seeding the database roles.");

    }

}

// --->>> FIM DO CÓDIGO PARA CRIAR ROLES <<<---

// --->>> SEED DATA <<<---
// Seed test data for development
if (app.Environment.IsDevelopment())
{
    try
    {
        await SeedData.SeedAsync(app.Services);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
// --->>> FIM DO SEED DATA <<<---





// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())

{

    app.UseWebAssemblyDebugging();

    app.UseMigrationsEndPoint();



    // Swagger Middleware

    app.UseSwagger();

    app.UseSwaggerUI(c =>

    {

        c.SwaggerEndpoint("/swagger/v1/swagger.json", "esii_2025_d2 API V1");

        // c.RoutePrefix = string.Empty; // Uncomment to serve Swagger UI at root

    });

}

else

{

    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    app.UseHsts();

}



app.UseHttpsRedirection();



app.UseStaticFiles(); // Certifique-se que isto está antes de UseAntiforgery se aplicável

app.UseAntiforgery();



// --->>> MAP CONTROLLER ROUTES <<<---

// Mapeia rotas para API Controllers ANTES de mapear componentes Blazor

app.MapControllers(); // Garante que as rotas da API são reconhecidas



app.MapRazorComponents<App>()

  .AddInteractiveServerRenderMode()

  .AddInteractiveWebAssemblyRenderMode()

  .AddAdditionalAssemblies(typeof(esii_2025_d2.Client._Imports).Assembly);



// Add additional endpoints required by the Identity /Account Razor components.

app.MapAdditionalIdentityEndpoints();





app.Run();
