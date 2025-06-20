using esii_2025_d2.Models;
using System.Threading.Tasks;

namespace esii_2025_d2.Services
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerByUserIdAsync(string userId);
        Task<(bool Success, string? ErrorMessage)> SaveCustomerAsync(Customer customer);
        Task<List<Customer>> GetAllCustomersAsync();
    }
}
