using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using esii_2025_d2.Models;

namespace esii_2025_d2.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Skills if empty
            if (!await context.Skills.AnyAsync())
            {
                var skills = new List<Skill>
                {
                    new Skill { Name = "JavaScript", Area = "Frontend" },
                    new Skill { Name = "React", Area = "Frontend" },
                    new Skill { Name = "Angular", Area = "Frontend" },
                    new Skill { Name = "Vue.js", Area = "Frontend" },
                    new Skill { Name = "C#", Area = "Backend" },
                    new Skill { Name = "ASP.NET Core", Area = "Backend" },
                    new Skill { Name = "Node.js", Area = "Backend" },
                    new Skill { Name = "Python", Area = "Backend" },
                    new Skill { Name = "Java", Area = "Backend" },
                    new Skill { Name = "SQL Server", Area = "Database" },
                    new Skill { Name = "PostgreSQL", Area = "Database" },
                    new Skill { Name = "MongoDB", Area = "Database" },
                    new Skill { Name = "Docker", Area = "DevOps" },
                    new Skill { Name = "Kubernetes", Area = "DevOps" },
                    new Skill { Name = "AWS", Area = "Cloud" },
                    new Skill { Name = "Azure", Area = "Cloud" }
                };

                context.Skills.AddRange(skills);
                await context.SaveChangesAsync();
            }

            // Seed Talent Categories if empty
            if (!await context.TalentCategories.AnyAsync())
            {
                var categories = new List<TalentCategory>
                {
                    new TalentCategory { Name = "Frontend Developer" },
                    new TalentCategory { Name = "Backend Developer" },
                    new TalentCategory { Name = "Full Stack Developer" },
                    new TalentCategory { Name = "DevOps Engineer" },
                    new TalentCategory { Name = "Data Scientist" },
                    new TalentCategory { Name = "Mobile Developer" },
                    new TalentCategory { Name = "UI/UX Designer" }
                };

                context.TalentCategories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Create test users if they don't exist
            var testTalentUser = await userManager.FindByEmailAsync("talent@test.com");
            if (testTalentUser == null)
            {
                testTalentUser = new ApplicationUser
                {
                    UserName = "talent@test.com",
                    Email = "talent@test.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(testTalentUser, "Test123!");
                await userManager.AddToRoleAsync(testTalentUser, "Talent");
            }

            var testCustomerUser = await userManager.FindByEmailAsync("customer@test.com");
            if (testCustomerUser == null)
            {
                testCustomerUser = new ApplicationUser
                {
                    UserName = "customer@test.com",
                    Email = "customer@test.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(testCustomerUser, "Test123!");
                await userManager.AddToRoleAsync(testCustomerUser, "Customer");
            }

            // Seed test customer if not exists
            if (!await context.Customers.AnyAsync())
            {
                var customers = new List<Customer>
                {
                    new Customer
                    {
                        Id = Guid.NewGuid().ToString(),
                        Company = "Tech Solutions Inc",
                        PhoneNumber = "+1234567890",
                        UserId = testCustomerUser.Id
                    },
                    new Customer
                    {
                        Id = Guid.NewGuid().ToString(),
                        Company = "Digital Innovations Ltd",
                        PhoneNumber = "+1234567891",
                        UserId = testCustomerUser.Id
                    }
                };

                context.Customers.AddRange(customers);
                await context.SaveChangesAsync();
            }

            // Seed test talents if not exists
            if (!await context.Talents.AnyAsync())
            {
                var frontendCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Frontend Developer");
                var backendCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Backend Developer");
                var fullstackCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Full Stack Developer");

                var talents = new List<Talent>
                {
                    new Talent
                    {
                        Name = "João Silva",
                        Country = "Portugal",
                        Email = "joao.silva@email.com",
                        HourlyRate = 45.00m,
                        IsPublic = true,
                        TalentCategoryId = frontendCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "Maria Santos",
                        Country = "Brazil",
                        Email = "maria.santos@email.com",
                        HourlyRate = 50.00m,
                        IsPublic = true,
                        TalentCategoryId = backendCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "Carlos Rodriguez",
                        Country = "Spain",
                        Email = "carlos.rodriguez@email.com",
                        HourlyRate = 55.00m,
                        IsPublic = true,
                        TalentCategoryId = fullstackCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "Ana Costa",
                        Country = "Portugal",
                        Email = "ana.costa@email.com",
                        HourlyRate = 40.00m,
                        IsPublic = true,
                        TalentCategoryId = frontendCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "Pedro Oliveira",
                        Country = "Brazil",
                        Email = "pedro.oliveira@email.com",
                        HourlyRate = 60.00m,
                        IsPublic = true,
                        TalentCategoryId = backendCategory.Id,
                        UserId = testTalentUser.Id
                    }
                };

                context.Talents.AddRange(talents);
                await context.SaveChangesAsync();

                // Add skills to talents
                var reactSkill = await context.Skills.FirstAsync(s => s.Name == "React");
                var jsSkill = await context.Skills.FirstAsync(s => s.Name == "JavaScript");
                var csharpSkill = await context.Skills.FirstAsync(s => s.Name == "C#");
                var nodeSkill = await context.Skills.FirstAsync(s => s.Name == "Node.js");

                var talentSkills = new List<TalentSkill>
                {
                    new TalentSkill { TalentId = talents[0].Id, SkillId = reactSkill.Id },
                    new TalentSkill { TalentId = talents[0].Id, SkillId = jsSkill.Id },
                    new TalentSkill { TalentId = talents[1].Id, SkillId = csharpSkill.Id },
                    new TalentSkill { TalentId = talents[2].Id, SkillId = reactSkill.Id },
                    new TalentSkill { TalentId = talents[2].Id, SkillId = nodeSkill.Id },
                    new TalentSkill { TalentId = talents[3].Id, SkillId = reactSkill.Id },
                    new TalentSkill { TalentId = talents[4].Id, SkillId = csharpSkill.Id }
                };

                context.TalentSkills.AddRange(talentSkills);
                await context.SaveChangesAsync();
            }

            // Seed job proposals if not exists
            if (!await context.JobProposals.AnyAsync())
            {
                var customer = await context.Customers.FirstAsync();
                var reactSkill = await context.Skills.FirstAsync(s => s.Name == "React");
                var csharpSkill = await context.Skills.FirstAsync(s => s.Name == "C#");
                var nodeSkill = await context.Skills.FirstAsync(s => s.Name == "Node.js");
                var frontendCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Frontend Developer");
                var backendCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Backend Developer");

                var jobProposals = new List<JobProposal>
                {
                    new JobProposal
                    {
                        Name = "E-commerce Website Frontend",
                        Description = "Build a modern React-based e-commerce frontend with responsive design and excellent UX.",
                        TotalHours = 120,
                        SkillId = reactSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = frontendCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "API Development with C#",
                        Description = "Develop a RESTful API using ASP.NET Core for a financial application.",
                        TotalHours = 80,
                        SkillId = csharpSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = backendCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "Node.js Microservices",
                        Description = "Create a microservices architecture using Node.js and Docker.",
                        TotalHours = 160,
                        SkillId = nodeSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = backendCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "React Dashboard Development",
                        Description = "Build an admin dashboard with React, charts, and real-time data.",
                        TotalHours = 100,
                        SkillId = reactSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = frontendCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "Mobile App Backend",
                        Description = "Develop backend services for a mobile application using C# and SQL Server.",
                        TotalHours = 140,
                        SkillId = csharpSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = backendCategory.Id
                    }
                };

                context.JobProposals.AddRange(jobProposals);
                await context.SaveChangesAsync();
            }
        }
    }
}
