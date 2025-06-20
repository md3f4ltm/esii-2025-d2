using NUnit.Framework;
using esii_2025_d2.Models;

namespace esii_2025_d2.Tests.Models;

[TestFixture]
public class ExperienceTests
{
    [Test]
    public void Experience_Creation_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var experience = new Experience
        {
            Id = 1,
            Title = "Software Developer",
            CompanyName = "Tech Company",
            StartYear = 2020,
            EndYear = 2023,
            TalentId = 1
        };

        // Assert
        Assert.That(experience.Id, Is.EqualTo(1));
        Assert.That(experience.Title, Is.EqualTo("Software Developer"));
        Assert.That(experience.CompanyName, Is.EqualTo("Tech Company"));
        Assert.That(experience.StartYear, Is.EqualTo(2020));
        Assert.That(experience.EndYear, Is.EqualTo(2023));
        Assert.That(experience.TalentId, Is.EqualTo(1));
    }

    [Test]
    public void IsEndYearValid_WhenEndYearIsNull_ShouldReturnTrue()
    {
        // Arrange
        var experience = new Experience
        {
            StartYear = 2020,
            EndYear = null
        };

        // Act
        var result = experience.IsEndYearValid();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsEndYearValid_WhenEndYearIsGreaterThanStartYear_ShouldReturnTrue()
    {
        // Arrange
        var experience = new Experience
        {
            StartYear = 2020,
            EndYear = 2023
        };

        // Act
        var result = experience.IsEndYearValid();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsEndYearValid_WhenEndYearIsEqualToStartYear_ShouldReturnTrue()
    {
        // Arrange
        var experience = new Experience
        {
            StartYear = 2020,
            EndYear = 2020
        };

        // Act
        var result = experience.IsEndYearValid();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsEndYearValid_WhenEndYearIsLessThanStartYear_ShouldReturnFalse()
    {
        // Arrange
        var experience = new Experience
        {
            StartYear = 2020,
            EndYear = 2019
        };

        // Act
        var result = experience.IsEndYearValid();

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsOverlapping_WithNoOtherExperiences_ShouldReturnFalse()
    {
        // Arrange
        var experience = new Experience
        {
            Id = 1,
            TalentId = 1,
            StartYear = 2020,
            EndYear = 2023
        };

        var otherExperiences = new List<Experience>();

        // Act
        var result = experience.IsOverlapping(otherExperiences);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsOverlapping_WithNonOverlappingExperiences_ShouldReturnFalse()
    {
        // Arrange
        var experience = new Experience
        {
            Id = 1,
            TalentId = 1,
            StartYear = 2020,
            EndYear = 2023
        };

        var otherExperiences = new List<Experience>
        {
            new Experience { Id = 2, TalentId = 1, StartYear = 2015, EndYear = 2018 },
            new Experience { Id = 3, TalentId = 1, StartYear = 2024, EndYear = 2025 }
        };

        // Act
        var result = experience.IsOverlapping(otherExperiences);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsOverlapping_WithOverlappingExperiences_ShouldReturnTrue()
    {
        // Arrange
        var experience = new Experience
        {
            Id = 1,
            TalentId = 1,
            StartYear = 2020,
            EndYear = 2023
        };

        var otherExperiences = new List<Experience>
        {
            new Experience { Id = 2, TalentId = 1, StartYear = 2022, EndYear = 2025 }
        };

        // Act
        var result = experience.IsOverlapping(otherExperiences);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsOverlapping_WithCurrentJobAndOverlappingPastJob_ShouldReturnTrue()
    {
        // Arrange
        var currentExperience = new Experience
        {
            Id = 1,
            TalentId = 1,
            StartYear = 2020,
            EndYear = null // Current job
        };

        var otherExperiences = new List<Experience>
        {
            new Experience { Id = 2, TalentId = 1, StartYear = 2022, EndYear = 2025 }
        };

        // Act
        var result = currentExperience.IsOverlapping(otherExperiences);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsOverlapping_WithDifferentTalentId_ShouldReturnFalse()
    {
        // Arrange
        var experience = new Experience
        {
            Id = 1,
            TalentId = 1,
            StartYear = 2020,
            EndYear = 2023
        };

        var otherExperiences = new List<Experience>
        {
            new Experience { Id = 2, TalentId = 2, StartYear = 2022, EndYear = 2025 } // Different talent
        };

        // Act
        var result = experience.IsOverlapping(otherExperiences);

        // Assert
        Assert.That(result, Is.False);
    }
}
