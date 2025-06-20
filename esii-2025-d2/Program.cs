using esii_2025_d2.Client.Pages;
using esii_2025_d2.Components;
using esii_2025_d2.Components.Account;
using esii_2025_d2.Data;
using esii_2025_d2.Models;
using esii_2025_d2.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// --- Adicionar serviços ao contentor ---

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

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

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();




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



builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CookieHandler>();

// Registar o HttpClient para uso nos componentes Blazor
builder.Services.AddHttpClient("API", (serviceProvider, client) =>
{
    // A alteração chave está aqui. Usamos o IHttpContextAccessor para
    // descobrir o endereço base da aplicação a partir do pedido atual do utilizador.
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    if (request != null)
    {
        client.BaseAddress = new Uri($"{request.Scheme}://{request.Host}");
    }
})
.AddHttpMessageHandler<CookieHandler>();

// Criar uma factory para que os componentes possam obter o HttpClient configurado
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// --- FIM DA CORREÇÃO DO HTTPCLIENT ---
// ==========================================================================================

// Registar os seus serviços de aplicação
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITalentService, TalentService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<IJobProposalService, JobProposalService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ITalentCategoryService, TalentCategoryService>();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "esii_2025_d2 API", Version = "v1" });
});

// --- Construção da Aplicação ---
var app = builder.Build();

// --- Seed de Roles e Dados ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // ... (O seu código de seed de roles continua aqui)
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "Admin", "Talent", "Customer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database roles.");
    }
}
if (app.Environment.IsDevelopment())
{
    // ... (O seu código de seed de dados continua aqui)
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

// --- Configuração do Pipeline de Pedidos HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "esii_2025_d2 API V1"));
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads"
});

app.UseRouting(); // Adicionar UseRouting se não existir
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery(); // Mover UseAntiforgery para depois da autorização

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(esii_2025_d2.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

app.Run();

// A classe do Cookie Handler no final do ficheiro
public class CookieHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CookieHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".AspNetCore.Identity.Application", out var cookieValue))
        {
            request.Headers.Add("Cookie", $".AspNetCore.Identity.Application={cookieValue}");
        }
        return base.SendAsync(request, cancellationToken);
    }
}