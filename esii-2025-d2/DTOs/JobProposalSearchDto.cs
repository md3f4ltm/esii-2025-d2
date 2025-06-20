using System.ComponentModel.DataAnnotations;

namespace esii_2025_d2.DTOs;

public class JobProposalSearchDto
{
    /// <summary>
    /// Search text that will be matched against Name and Description
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// Filter by specific skill ID
    /// </summary>
    public int? SkillId { get; set; }

    /// <summary>
    /// Filter by specific talent category ID
    /// </summary>
    public int? TalentCategoryId { get; set; }

    /// <summary>
    /// Filter by specific customer ID
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Minimum total hours for the job
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum hours must be a positive number")]
    public int? MinTotalHours { get; set; }

    /// <summary>
    /// Maximum total hours for the job
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum hours must be a positive number")]
    public int? MaxTotalHours { get; set; }

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
