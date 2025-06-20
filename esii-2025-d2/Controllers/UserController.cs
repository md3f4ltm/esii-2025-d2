using esii_2025_d2.Data;
using esii_2025_d2.Models;
using esii_2025_d2.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace esii_2025_d2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        // O construtor está mais simples agora, sem o IUserProfileService
        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUsers()
        {
            return await _context.Users
                .Select(u => new UserInfoDto { Id = u.Id, Email = u.Email, Username = u.UserName, Name = u.Name })
                .ToListAsync();
        }

        // O método GetUser volta a ter a lógica dentro dele, o que é perfeitamente aceitável
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserProfileDto>> GetUser(string id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Skills)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            var userProfile = new UserProfileDto
            {
                Id = user.Id, Email = user.Email, Username = user.UserName, Name = user.Name, Description = user.Description,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Skills = user.Skills.Select(s => new SkillDto { Id = s.Id, Name = s.Name, Area = s.Area }).ToList(),
                Area = user.Area, Roles = roles
            };
            return Ok(userProfile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto updatedUserDto)
        {
            if (id != updatedUserDto.Id) return BadRequest("ID mismatch.");
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId != id && !User.IsInRole("Admin")) return Forbid();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.Name = updatedUserDto.Name;
            user.Description = updatedUserDto.Description;
            user.Area = updatedUserDto.Area;
            var updateResult = await _userManager.UpdateAsync(user);
            return !updateResult.Succeeded ? BadRequest(updateResult.Errors) : NoContent();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("{userId}/avatar")]
        public async Task<IActionResult> UploadAvatar(string userId, IFormFile file)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId != userId && !User.IsInRole("Admin")) return Forbid();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
            var uploadsFolderPath = Path.Combine(_env.WebRootPath, "uploads", "avatars");
            if (!Directory.Exists(uploadsFolderPath)) Directory.CreateDirectory(uploadsFolderPath);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolderPath, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }
            user.ProfilePictureUrl = $"/uploads/avatars/{fileName}";
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Ok(user.ProfilePictureUrl) : BadRequest("Failed to update user profile picture.");
        }

        [HttpPost("{userId}/skills")]
        public async Task<IActionResult> AddSkillToUser(string userId, [FromBody] SkillDto skillDto)
        {
            var user = await _context.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound("User not found");
            if (user.Skills.Any(s => s.Id == skillDto.Id)) return BadRequest("User already has this skill.");
            var skillToAdd = await _context.Skills.FindAsync(skillDto.Id);
            if (skillToAdd == null) return NotFound("Skill not found in the database.");
            user.Skills.Add(skillToAdd);
            await _context.SaveChangesAsync();
            return Ok(new SkillDto { Id = skillToAdd.Id, Name = skillToAdd.Name, Area = skillToAdd.Area });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId != id && !User.IsInRole("Admin")) return Forbid();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var result = await _userManager.DeleteAsync(user);
            return !result.Succeeded ? BadRequest(new { message = "Failed to delete user.", errors = result.Errors.Select(e => e.Description) }) : NoContent();
        }
    }
}