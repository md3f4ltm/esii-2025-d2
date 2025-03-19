using BlazorApp1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<int> GetUserCountAsync();
        Task<bool> CreateTestUserAsync();
        Task<bool> CheckConnectionAsync();
        Task<DatabaseConnectionInfo> GetConnectionDetailsAsync();
    }
}