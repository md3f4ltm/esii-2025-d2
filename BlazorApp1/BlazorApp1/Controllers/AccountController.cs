using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ESII2025d2.Models;
using ESII2025d2.Models.Dtos; // Use DTOs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Needed for Authorize attribute
using System.Security.Claims; // To get user info

namespace ESII2025d2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Utilizador> _userManager;
        private readonly SignInManager<Utilizador> _signInManager;
        private readonly ILogger<AccountController> _logger; // Optional: for logging

        public AccountController(
            UserManager<Utilizador> userManager,
            SignInManager<Utilizador> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Utilizador
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                nome = registerDto.Nome, // Set custom property
                datanascimento = registerDto.DataNascimento // Set custom property
                // EmailConfirmed can be set to true for simplicity, or implement confirmation flow
                // EmailConfirmed = true
            };

            // UserManager handles password hashing
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                 _logger?.LogInformation("User {Username} created successfully.", user.UserName);
                // Optional: Sign in the user immediately after registration
                // await _signInManager.SignInAsync(user, isPersistent: false);
                // Return user info or just success
                 return Ok(new { Message = "Registration successful" });
               // return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user); // Need a GetUser method
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _logger?.LogWarning("User {Username} registration failed: {Errors}", user.UserName, result.Errors);
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Attempt to sign in using PasswordSignInAsync
            // Set lockoutOnFailure to true in production for security
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Email, // Or loginDto.Username if allowing username login
                loginDto.Password,
                loginDto.RememberMe, // Use value from DTO for persistent cookie
                lockoutOnFailure: false); // Consider true for production

            if (result.Succeeded)
            {
                _logger?.LogInformation("User {Email} logged in successfully.", loginDto.Email);
                 var user = await _userManager.FindByEmailAsync(loginDto.Email); // Or FindByNameAsync if using username
                 if(user == null) return Unauthorized("Login failed."); // Should not happen if SignIn succeeded but safety check

                 var userInfo = new UserInfoDto // Return some basic info
                 {
                     Id = user.Id,
                     Email = user.Email,
                     Username = user.UserName,
                     Nome = user.nome
                 };
                return Ok(userInfo); // Send back some user info
            }

            if (result.IsLockedOut)
            {
                 _logger?.LogWarning("User {Email} account locked out.", loginDto.Email);
                return Unauthorized("Account locked out.");
            }
            else if (result.IsNotAllowed)
            {
                 _logger?.LogWarning("User {Email} is not allowed to sign in.", loginDto.Email);
                return Unauthorized("Login not allowed."); // E.g., requires email confirmation
            }
            else // Handles incorrect password
            {
                 _logger?.LogWarning("Invalid login attempt for {Email}.", loginDto.Email);
                return Unauthorized("Invalid login attempt.");
            }
        }

        [HttpPost("logout")]
        [Authorize] // Only logged-in users can logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
             _logger?.LogInformation("User logged out successfully.");
            return Ok(new { Message = "Logout successful" });
        }

        // Optional: Get current user info endpoint
        [HttpGet("userinfo")]
        [Authorize] // Ensure user is logged in
        public async Task<ActionResult<UserInfoDto>> GetUserInfo()
        {
            // Find user based on the current authentication cookie
            var user = await _userManager.GetUserAsync(User); // User comes from ControllerBase

            if (user == null)
            {
                return NotFound("User not found."); // Should not happen if [Authorize] works
            }

            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Nome = user.nome
            };
            return Ok(userInfo);
        }
    }
}
