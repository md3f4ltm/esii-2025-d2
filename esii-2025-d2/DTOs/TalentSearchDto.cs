using System.ComponentModel.DataAnnotations;

namespace esii_2025_d2.DTOs;

public class TalentSearchDto
{
    /// <summary>
    /// Search text that will be matched against Name and Country
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// Filter by specific talent category ID
    /// </summary>
    public int? TalentCategoryId { get; set; }

    /// <summary>
    /// Filter by specific skill ID
    /// </summary>
    public int? SkillId { get; set; }

    /// <summary>
    /// Filter by country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Minimum hourly rate
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Minimum hourly rate must be a positive number")]
    public decimal? MinHourlyRate { get; set; }

    /// <summary>
    /// Maximum hourly rate
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Maximum hourly rate must be a positive number")]
    public decimal? MaxHourlyRate { get; set; }

    /// <summary>
    /// Filter by public status (null = all, true = public only, false = private only)
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Field to sort by
    /// </summary>
    public string? SortBy { get; set; } = "Name";

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}
