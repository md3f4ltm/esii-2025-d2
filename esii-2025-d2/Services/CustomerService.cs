using esii_2025_d2.Data; // Assuming ApplicationDbContext is here
using esii_2025_d2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace esii_2025_d2.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByUserIdAsync(string userId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<(bool Success, string? ErrorMessage)> SaveCustomerAsync(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.UserId))
            {
                return (false, "User ID is required.");
            }

            try
            {
                var existingCustomer = await _context.Customers
                                                 .FirstOrDefaultAsync(c => c.UserId == customer.UserId);

                if (existingCustomer == null)
                {
                    // New customer record
                    if (string.IsNullOrWhiteSpace(customer.Id) || customer.Id == string.Empty) // Check if Id is empty for new record
                    {
                         customer.Id = Guid.NewGuid().ToString(); // Generate new Id for new customer
                    }
                    _context.Customers.Add(customer);
                }
                else
                {
                    // Update existing customer record
                    existingCustomer.Company = customer.Company;
                    existingCustomer.PhoneNumber = customer.PhoneNumber;
                    // Do not update existingCustomer.Id or existingCustomer.UserId
                    _context.Customers.Update(existingCustomer);
                }

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (ex) if necessary
                return (false, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
