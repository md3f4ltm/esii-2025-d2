using Microsoft.AspNetCore.Identity;

namespace BlazorApp1.Models;

public class ApplicationUser : IdentityUser
{
    // Add custom user properties here
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}