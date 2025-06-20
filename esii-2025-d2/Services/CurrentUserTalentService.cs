using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using esii_2025_d2.Data;
using esii_2025_d2.Models;

namespace esii_2025_d2.Services
{
    /// <summary>
    /// Service specifically designed for Blazor components to handle current user's talents.
    /// This service automatically manages user authentication context and ensures data isolation.
    /// </summary>
    public interface ICurrentUserTalentService
    {
        Task<List<Talent>> GetMyTalentsAsync();
        Task<Talent> CreateTalentAsync(Talent talent);
        Task<Talent> UpdateTalentAsync(Talent talent);
        Task DeleteTalentAsync(int id);
        Task<Talent?> GetTalentByIdAsync(int id);
    }

    public class CurrentUserTalentService : ICurrentUserTalentService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authStateProvider;

        public CurrentUserTalentService(ApplicationDbContext context, AuthenticationStateProvider authStateProvider)
        {
            _context = context;
            _authStateProvider = authStateProvider;
        }

        /// <summary>
        /// Gets the current authenticated user's ID from the authentication state.
        /// </summary>
        /// <returns>The user ID of the currently authenticated user.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        private async Task<string> GetCurrentUserIdAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or user ID not found.");
            }

            return userId;
        }

        /// <summary>
        /// Gets all talents belonging to the currently authenticated user.
        /// </summary>
        /// <returns>A list of talents owned by the current user, including their talent categories, ordered by name.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        public async Task<List<Talent>> GetMyTalentsAsync()
        {
            var userId = await GetCurrentUserIdAsync();

            return await _context.Talents
                .Include(t => t.TalentCategory)
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new talent for the current user.
        /// </summary>
        /// <param name="talent">The talent to create. UserId will be automatically set to the current user.</param>
        /// <returns>The created talent.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        public async Task<Talent> CreateTalentAsync(Talent talent)
        {
            var userId = await GetCurrentUserIdAsync();

            // Always set the UserId to current user to prevent tampering
            talent.UserId = userId;

            _context.Talents.Add(talent);
            await _context.SaveChangesAsync();

            // Reload with TalentCategory included
            return await _context.Talents
                .Include(t => t.TalentCategory)
                .FirstAsync(t => t.Id == talent.Id);
        }

        /// <summary>
        /// Updates an existing talent. Only the owner can update their talents.
        /// </summary>
        /// <param name="talent">The talent to update.</param>
        /// <returns>The updated talent.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated or doesn't own the talent.</exception>
        public async Task<Talent> UpdateTalentAsync(Talent talent)
        {
            var userId = await GetCurrentUserIdAsync();

            // Ensure the talent belongs to the current user
            var existingTalent = await _context.Talents
                .FirstOrDefaultAsync(t => t.Id == talent.Id && t.UserId == userId);

            if (existingTalent == null)
            {
                throw new UnauthorizedAccessException("Talent not found or you don't have permission to modify it.");
            }

            // Always set the UserId to current user to prevent tampering
            talent.UserId = userId;

            _context.Entry(existingTalent).CurrentValues.SetValues(talent);
            await _context.SaveChangesAsync();

            // Reload with TalentCategory included
            return await _context.Talents
                .Include(t => t.TalentCategory)
                .FirstAsync(t => t.Id == talent.Id);
        }

        /// <summary>
        /// Deletes a talent. Only the owner can delete their talents.
        /// </summary>
        /// <param name="id">The ID of the talent to delete.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated or doesn't own the talent.</exception>
        public async Task DeleteTalentAsync(int id)
        {
            var userId = await GetCurrentUserIdAsync();

            var talent = await _context.Talents
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (talent == null)
            {
                throw new UnauthorizedAccessException("Talent not found or you don't have permission to delete it.");
            }

            _context.Talents.Remove(talent);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a specific talent by ID. Only the owner can access their talents.
        /// </summary>
        /// <param name="id">The ID of the talent to retrieve.</param>
        /// <returns>The talent if found and owned by the current user, null otherwise.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        public async Task<Talent?> GetTalentByIdAsync(int id)
        {
            var userId = await GetCurrentUserIdAsync();

            return await _context.Talents
                .Include(t => t.TalentCategory)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }
    }
}
