using BlazorApp1.Data;
using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
            return await _userService.GetAllUsersAsync();
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetUserCount()
        {
            return await _userService.GetUserCountAsync();
        }

        [HttpPost("test-user")]
        public async Task<ActionResult<bool>> CreateTestUser()
        {
            return await _userService.CreateTestUserAsync();
        }

        [HttpGet("connection-status")]
        public async Task<ActionResult<bool>> CheckConnection()
        {
            return await _userService.CheckConnectionAsync();
        }

        [HttpGet("debug-connection")]
        public async Task<ActionResult<DatabaseConnectionInfo>> GetConnectionDetails()
        {
            return await _userService.GetConnectionDetailsAsync();
        }
    }
}

