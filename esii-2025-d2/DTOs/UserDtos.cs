using System.Collections.Generic;

namespace esii_2025_d2.DTOs
{
    public class UserInfoDto
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string Name { get; set; } = null!;
    }

    public class UserProfileDto
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public ICollection<SkillDto> Skills { get; set; } = new List<SkillDto>();
        public string? Area { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class UserUpdateDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Area { get; set; }
    }

    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Area { get; set; } = null!;
    }
}