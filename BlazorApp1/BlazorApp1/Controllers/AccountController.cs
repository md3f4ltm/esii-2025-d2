using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ESII2025d2.Models;
using ESII2025d2.Models.Dtos; // Use DTOs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Linq; // Needed for SelectMany

namespace ESII2025d2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Utilizador> _userManager;
        private readonly SignInManager<Utilizador> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<Utilizador> userManager,
            SignInManager<Utilizador> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // Register endpoint - Stays the same
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // ... (Your existing registration logic) ...
             if (!ModelState.IsValid) { return ValidationProblem(ModelState); }
             var user = new Utilizador { UserName = registerDto.Email, Email = registerDto.Email /* Add other props */ };
             var result = await _userManager.CreateAsync(user, registerDto.Password);
             if (result.Succeeded) { return Ok(new { Message = "Registration successful" }); }
             foreach (var error in result.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
             return ValidationProblem(ModelState);
        }


        // Login endpoint - No longer used *by the Blazor login page*
        // Can be kept if other clients (e.g., mobile app, JavaScript client) need it
        [HttpPost("login")]
        [AllowAnonymous] 
        // [IgnoreAntiforgeryToken] // Only needed if called by non-browser clients (like HttpClient/fetch) that don't handle antiforgery cookies automatically.
                                    // Since the Blazor page isn't calling this anymore, it's less critical here unless other clients use it.
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
             _logger?.LogInformation("API /api/account/login called for email {Email}", loginDto.Email); // Log API call

             if (!ModelState.IsValid) { /* ... */ return ValidationProblem(ModelState); }
             var user = await _userManager.FindByEmailAsync(loginDto.Email);
             if (user == null) { /* ... */ return Unauthorized(new { ErrorCode = "InvalidCredentials", Message = "Invalid login attempt." }); }

             var result = await _signInManager.PasswordSignInAsync( user, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

             if (result.Succeeded) { /* ... */ return Ok(new { Message = "Login successful via API" }); } // Indicate API success
             if (result.IsLockedOut) { /* ... */ return Unauthorized(new { ErrorCode = "LockedOut", Message = "Account locked out." }); }
             else if (result.IsNotAllowed) { /* ... */ return Unauthorized(new { ErrorCode = "NotAllowed", Message = "Login not allowed (e.g., email not confirmed)." }); }
             else { /* ... */ return Unauthorized(new { ErrorCode = "InvalidCredentials", Message = "Invalid login attempt." }); }
        }

        // Logout endpoint - Stays the same, can be called from Blazor
        [HttpPost("logout")]
        [Authorize] // Ensure only logged-in users can logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger?.LogInformation("User logged out.");
             // It's often better to return NoContent() or Ok() for logout API calls
             // rather than relying on redirection from the API itself.
             // Let the client handle redirection after calling this.
            return Ok(new { Message = "Logout successful" });
        }

        // GetUserInfo endpoint - Stays the same
        [HttpGet("userinfo")]
        [Authorize]
        public async Task<ActionResult<UserInfoDto>> GetUserInfo()
        {
             var user = await _userManager.GetUserAsync(User);
             if (user == null) return NotFound("User not found.");
             // Populate UserInfoDto correctly based on your Utilizador properties
             var userInfo = new UserInfoDto { Email = user.Email /* Add other props */ };
             return Ok(userInfo);
        }
    }
}
