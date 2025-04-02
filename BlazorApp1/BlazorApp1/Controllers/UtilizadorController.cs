using ESII2025d2.Models;
using ESII2025d2.Models.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ESII2025d2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UtilizadorController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Utilizador> _userManager;

    public UtilizadorController(ApplicationDbContext context, UserManager<Utilizador> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/Utilizador
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUtilizadores()
    {
        return await _context.Users
            .Select(u => new UserInfoDto {
                Id = u.Id,
                Email = u.Email,
                Username = u.UserName,
                Nome = u.nome
            })
            .ToListAsync();
    }

    // GET: api/Utilizador/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserInfoDto>> GetUtilizador(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
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

    // PUT: api/Utilizador/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUtilizador(string id, UserInfoDto updatedUserDto)
    {
        if (id != updatedUserDto.Id)
        {
            return BadRequest("ID mismatch.");
        }

        var currentUserId = _userManager.GetUserId(User);
        // Uncomment if you implement role-based authorization
        // if (currentUserId != id && !User.IsInRole("Admin"))
        // {
        //     return Forbid();
        // }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Update custom properties
        user.nome = updatedUserDto.Nome;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach(var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        var talento = await _context.Talentos.FirstOrDefaultAsync(t => t.idutilizador.ToString() == id);
        if (talento != null)
        {
            talento.nome = user.nome;
            talento.email = user.Email;
            _context.Entry(talento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    // DELETE: api/Utilizador/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUtilizador(string id)
    {
        var currentUserId = _userManager.GetUserId(User);
        // Uncomment if you implement role-based authorization
        // if (currentUserId != id && !User.IsInRole("Admin"))
        // {
        //     return Forbid();
        // }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    // Helper method to check if user exists
    private bool UtilizadorExists(string id)
    {
        return _context.Users.Any(u => u.Id == id);
    }
}
