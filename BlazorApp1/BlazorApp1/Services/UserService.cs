using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _dbContext.Users.AsNoTracking().ToListAsync();
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _dbContext.Users.CountAsync();
        }

        public async Task<bool> CreateTestUserAsync()
        {
            try
            {
                // Check if test user already exists
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.UserName == "testuser@example.com");

                if (existingUser != null)
                {
                    return false; // User already exists
                }

                // Create a new test user
                var testUser = new ApplicationUser
                {
                    UserName = "testuser@example.com",
                    Email = "testuser@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    EmailConfirmed = true
                };

                _dbContext.Users.Add(testUser);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckConnectionAsync()
        {
            return await _dbContext.Database.CanConnectAsync();
        }

        public async Task<DatabaseConnectionInfo> GetConnectionDetailsAsync()
        {
            try
            {
                var connectionString = _dbContext.Database.GetConnectionString();
                // Mask password for security
                var maskedConnString = connectionString?.Contains("Password=") == true ?
                    connectionString.Replace(connectionString.Split("Password=")[1].Split(";")[0], "***MASKED***") :
                    connectionString;

                int sqlResult = 0;
                try
                {
                    sqlResult = await _dbContext.Database.ExecuteSqlRawAsync("SELECT COUNT(*) FROM \"AspNetUsers\"");
                }
                catch { /* Ignore errors here */ }

                var info = new DatabaseConnectionInfo
                {
                    ConnectionString = maskedConnString ?? string.Empty,
                    ContextType = _dbContext.GetType().FullName ?? string.Empty,
                    ContextInstanceId = _dbContext.GetHashCode(),
                    SqlQueryResult = sqlResult,
                    CanConnect = await _dbContext.Database.CanConnectAsync()
                };

                if (info.CanConnect)
                {
                    try
                    {
                        var sql = @"SELECT EXISTS(
                            SELECT 1 FROM pg_tables 
                            WHERE schemaname = 'public' 
                            AND tablename = 'AspNetUsers'
                        )";

                        info.TableExists = await _dbContext.Database.ExecuteSqlRawAsync(sql) > 0;
                        info.UserCount = await _dbContext.Users.CountAsync();
                    }
                    catch (Exception ex)
                    {
                        info.ErrorMessage = ex.Message;
                    }
                }

                return info;
            }
            catch (Exception ex)
            {
                return new DatabaseConnectionInfo
                {
                    CanConnect = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    public class DatabaseConnectionInfo
    {
        public string? ConnectionString { get; set; }
        public string? ContextType { get; set; }
        public int ContextInstanceId { get; set; }
        public int SqlQueryResult { get; set; }
        public bool CanConnect { get; set; }
        public bool TableExists { get; set; }
        public int UserCount { get; set; }
        public string? ErrorMessage { get; set; }
    }
}