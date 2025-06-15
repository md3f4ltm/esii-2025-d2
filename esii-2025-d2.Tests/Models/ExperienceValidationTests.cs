using System.ComponentModel.DataAnnotations;

namespace esii_2025_d2.Tests.Models;

[TestFixture]
public class ExperienceValidationTests
{
    [Test]
    public void Experience_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var experience = new Experience
        {
            Title = "Software Developer",
            CompanyName = "Tech Company",
            StartYear = 2020,
            EndYear = 2023,
            TalentId = 1
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void Experience_WithEmptyTitle_ShouldFailValidation()
    {
        // Arrange
        var experience = new Experience
        {
            Title = "", // Empty title
            CompanyName = "Tech Company",
            StartYear = 2020,
            TalentId = 1
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Title")), Is.True);
    }

    [Test]
    public void Experience_WithEmptyCompanyName_ShouldFailValidation()
    {
        // Arrange
        var experience = new Experience
        {
            Title = "Developer",
            CompanyName = "", // Empty company name
            StartYear = 2020,
            TalentId = 1
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("CompanyName")), Is.True);
    }

    [Test]
    public void Experience_WithInvalidStartYear_ShouldFailValidation()
    {
        // Arrange
        var experience = new Experience
        {
            Title = "Developer",
            CompanyName = "Company",
            StartYear = 1800, // Invalid year (too early)
            TalentId = 1
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("StartYear")), Is.True);
    }

    [Test]
    public void Experience_WithInvalidEndYear_ShouldFailValidation()
    {
        // Arrange
        var experience = new Experience
        {
            Title = "Developer",
            CompanyName = "Company",
            StartYear = 2020,
            EndYear = 2200, // Invalid year (too far in future)
            TalentId = 1
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("EndYear")), Is.True);
    }

    [Test]
    public void Experience_WithTooLongTitle_ShouldFailValidation()
    {
        // Arrange
        var longTitle = new string('A', 201); // 201 characters (exceeds StringLength(200))
        var experience = new Experience
        {
            Title = longTitle,
            CompanyName = "Company",
            StartYear = 2020,
            TalentId = 1
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Title")), Is.True);
    }

    [Test]
    public void Experience_WithTooLongCompanyName_ShouldFailValidation()
    {
        // Arrange
        var longCompanyName = new string('B', 151); // 151 characters (exceeds StringLength(150))
        var experience = new Experience
        {
            Title = "Developer",
            CompanyName = longCompanyName,
            StartYear = 2020,
            TalentId = 1
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("CompanyName")), Is.True);
    }

    [Test]
    public void Experience_WithZeroTalentId_ShouldPassValidation()
    {
        // Arrange
        var experience = new Experience
        {
            Title = "Developer",
            CompanyName = "Company",
            StartYear = 2020,
            TalentId = 0 // Zero is actually valid for integer - only Required attribute would fail this
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        // TalentId = 0 doesn't fail basic validation since it's just an int
        // The foreign key validation would happen at the database level
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void Experience_WithValidBoundaryValues_ShouldPassValidation()
    {
        // Arrange
        var experience = new Experience
        {
            Title = new string('A', 200), // Exactly 200 characters
            CompanyName = new string('B', 150), // Exactly 150 characters
            StartYear = 1900, // Minimum valid year
            EndYear = 2100, // Maximum valid year
            TalentId = 1
        };

        // Act
        var validationResults = ValidateModel(experience);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, validationResults, true);
        return validationResults;
    }
}
