using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using esii_2025_d2.Controllers;
using esii_2025_d2.Models;
using esii_2025_d2.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esii_2025_d2.Tests.Controllers;

[TestFixture]
public class ExperienceControllerTests
{
    private ApplicationDbContext _context;
    private ExperienceController _controller;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new ExperienceController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetExperiences_ShouldReturnAllExperiences()
    {
        // Arrange
        var experiences = new List<Experience>
        {
            new Experience { Title = "Developer", CompanyName = "Company A", StartYear = 2020, TalentId = 1 },
            new Experience { Title = "Analyst", CompanyName = "Company B", StartYear = 2021, TalentId = 2 }
        };

        _context.Experiences.AddRange(experiences);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetExperiences();

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetExperience_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.GetExperience(999);

        // Assert
        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task CreateExperience_WithInvalidTalentId_ShouldReturnBadRequest()
    {
        // Arrange
        var newExperience = new Experience
        {
            Title = "Developer",
            CompanyName = "Company",
            StartYear = 2023,
            TalentId = 999 // Non-existent talent
        };

        // Act
        var result = await _controller.CreateExperience(newExperience);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateExperience_WithInvalidEndYear_ShouldReturnBadRequest()
    {
        // Arrange
        var talent = new Talent
        {
            UserId = "user1",
            Name = "John Doe",
            Country = "Portugal",
            Email = "john@example.com",
            HourlyRate = 25.00m,
            TalentCategoryId = 1
        };
        _context.Talents.Add(talent);
        await _context.SaveChangesAsync();

        var newExperience = new Experience
        {
            Title = "Developer",
            CompanyName = "Company",
            StartYear = 2023,
            EndYear = 2020, // End year before start year
            TalentId = talent.Id
        };

        // Act
        var result = await _controller.CreateExperience(newExperience);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task DeleteExperience_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var experience = new Experience
        {
            Title = "Developer",
            CompanyName = "Company",
            StartYear = 2020,
            TalentId = 1
        };

        _context.Experiences.Add(experience);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteExperience(experience.Id);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());

        // Verify experience was deleted
        var deletedExperience = await _context.Experiences.FindAsync(experience.Id);
        Assert.That(deletedExperience, Is.Null);
    }

    [Test]
    public async Task DeleteExperience_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.DeleteExperience(999);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task UpdateExperience_WithMismatchedId_ShouldReturnBadRequest()
    {
        // Arrange
        var updatedExperience = new Experience
        {
            Id = 2, // Different from the ID in the route
            Title = "Developer",
            CompanyName = "Company",
            StartYear = 2020,
            TalentId = 1
        };

        // Act
        var result = await _controller.UpdateExperience(1, updatedExperience);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateExperience_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var talent = new Talent
        {
            UserId = "user1",
            Name = "John Doe",
            Country = "Portugal",
            Email = "john@example.com",
            HourlyRate = 25.00m,
            TalentCategoryId = 1
        };
        _context.Talents.Add(talent);
        await _context.SaveChangesAsync();

        var newExperience = new Experience
        {
            Title = "Senior Developer",
            CompanyName = "Tech Corp",
            StartYear = 2023,
            EndYear = null,
            TalentId = talent.Id
        };

        // Act
        var result = await _controller.CreateExperience(newExperience);

        // Assert
        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());

        // Verify the experience was created in the database
        var createdExperience = await _context.Experiences.FirstOrDefaultAsync(e => e.Title == "Senior Developer");
        Assert.That(createdExperience, Is.Not.Null);
        Assert.That(createdExperience!.CompanyName, Is.EqualTo("Tech Corp"));
    }
}
