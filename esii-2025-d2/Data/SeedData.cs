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
                    // Frontend Technologies
                    new Skill { Name = "React", Area = "Frontend" },
                    new Skill { Name = "Next.js", Area = "Frontend" },
                    new Skill { Name = "Angular", Area = "Frontend" },
                    new Skill { Name = "Vue.js", Area = "Frontend" },
                    new Skill { Name = "TypeScript", Area = "Frontend" },
                    new Skill { Name = "JavaScript", Area = "Frontend" },
                    new Skill { Name = "Tailwind CSS", Area = "Frontend" },
                    new Skill { Name = "Sass/SCSS", Area = "Frontend" },
                    
                    // Backend Technologies
                    new Skill { Name = "C#", Area = "Backend" },
                    new Skill { Name = "ASP.NET Core", Area = "Backend" },
                    new Skill { Name = "Entity Framework", Area = "Backend" },
                    new Skill { Name = "Node.js", Area = "Backend" },
                    new Skill { Name = "Express.js", Area = "Backend" },
                    new Skill { Name = "Python", Area = "Backend" },
                    new Skill { Name = "Django", Area = "Backend" },
                    new Skill { Name = "FastAPI", Area = "Backend" },
                    new Skill { Name = "Java", Area = "Backend" },
                    new Skill { Name = "Spring Boot", Area = "Backend" },
                    new Skill { Name = "Go", Area = "Backend" },
                    new Skill { Name = "Rust", Area = "Backend" },
                    
                    // Database Technologies
                    new Skill { Name = "SQL Server", Area = "Database" },
                    new Skill { Name = "PostgreSQL", Area = "Database" },
                    new Skill { Name = "MySQL", Area = "Database" },
                    new Skill { Name = "MongoDB", Area = "Database" },
                    new Skill { Name = "Redis", Area = "Database" },
                    new Skill { Name = "Elasticsearch", Area = "Database" },
                    new Skill { Name = "Prisma", Area = "Database" },
                    
                    // DevOps & Cloud
                    new Skill { Name = "Docker", Area = "DevOps" },
                    new Skill { Name = "Kubernetes", Area = "DevOps" },
                    new Skill { Name = "Jenkins", Area = "DevOps" },
                    new Skill { Name = "GitHub Actions", Area = "DevOps" },
                    new Skill { Name = "Terraform", Area = "DevOps" },
                    new Skill { Name = "AWS", Area = "Cloud" },
                    new Skill { Name = "Azure", Area = "Cloud" },
                    new Skill { Name = "Google Cloud", Area = "Cloud" },
                    new Skill { Name = "Serverless", Area = "Cloud" },
                    
                    // Mobile Development
                    new Skill { Name = "React Native", Area = "Mobile" },
                    new Skill { Name = "Flutter", Area = "Mobile" },
                    new Skill { Name = "Xamarin", Area = "Mobile" },
                    new Skill { Name = "Swift", Area = "Mobile" },
                    new Skill { Name = "Kotlin", Area = "Mobile" },
                    
                    // AI & Data Science
                    new Skill { Name = "Machine Learning", Area = "AI/ML" },
                    new Skill { Name = "TensorFlow", Area = "AI/ML" },
                    new Skill { Name = "PyTorch", Area = "AI/ML" },
                    new Skill { Name = "Data Analysis", Area = "Data Science" },
                    new Skill { Name = "Pandas", Area = "Data Science" },
                    new Skill { Name = "NumPy", Area = "Data Science" },
                    
                    // Testing & Quality
                    new Skill { Name = "Jest", Area = "Testing" },
                    new Skill { Name = "Cypress", Area = "Testing" },
                    new Skill { Name = "Selenium", Area = "Testing" },
                    new Skill { Name = "Unit Testing", Area = "Testing" },
                    new Skill { Name = "TDD", Area = "Testing" },
                    
                    // Other IT Skills
                    new Skill { Name = "GraphQL", Area = "API" },
                    new Skill { Name = "REST APIs", Area = "API" },
                    new Skill { Name = "Microservices", Area = "Architecture" },
                    new Skill { Name = "Event-Driven Architecture", Area = "Architecture" },
                    new Skill { Name = "System Design", Area = "Architecture" },
                    new Skill { Name = "Agile/Scrum", Area = "Methodology" },
                    new Skill { Name = "Git", Area = "Version Control" },
                    new Skill { Name = "Linux", Area = "System Administration" }
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
                    new TalentCategory { Name = "Cloud Architect" },
                    new TalentCategory { Name = "Data Scientist" },
                    new TalentCategory { Name = "Machine Learning Engineer" },
                    new TalentCategory { Name = "Mobile Developer" },
                    new TalentCategory { Name = "UI/UX Designer" },
                    new TalentCategory { Name = "Software Architect" },
                    new TalentCategory { Name = "Database Administrator" },
                    new TalentCategory { Name = "Cybersecurity Specialist" },
                    new TalentCategory { Name = "QA Engineer" },
                    new TalentCategory { Name = "Product Manager" },
                    new TalentCategory { Name = "Technical Lead" }
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

                // Ensure email is confirmed
                var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(testTalentUser);
                await userManager.ConfirmEmailAsync(testTalentUser, confirmToken);

                // Disable two-factor authentication
                await userManager.SetTwoFactorEnabledAsync(testTalentUser, false);

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

                // Ensure email is confirmed
                var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(testCustomerUser);
                await userManager.ConfirmEmailAsync(testCustomerUser, confirmToken);

                // Disable two-factor authentication
                await userManager.SetTwoFactorEnabledAsync(testCustomerUser, false);

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
                        Company = "TechFlow Solutions",
                        PhoneNumber = "+351 213 456 789",
                        UserId = testCustomerUser.Id
                    },
                    new Customer
                    {
                        Id = Guid.NewGuid().ToString(),
                        Company = "CloudSync Innovations",
                        PhoneNumber = "+351 214 567 890",
                        UserId = testCustomerUser.Id
                    },
                    new Customer
                    {
                        Id = Guid.NewGuid().ToString(),
                        Company = "DataMind Technologies",
                        PhoneNumber = "+351 215 678 901",
                        UserId = testCustomerUser.Id
                    },
                    new Customer
                    {
                        Id = Guid.NewGuid().ToString(),
                        Company = "NextGen Software",
                        PhoneNumber = "+351 216 789 012",
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
                var devopsCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "DevOps Engineer");
                var mobileCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Mobile Developer");

                var talents = new List<Talent>
                {
                    new Talent
                    {
                        Name = "Ana Rodrigues",
                        Country = "Portugal",
                        Email = "ana.rodrigues@techmail.com",
                        HourlyRate = 65.00m,
                        IsPublic = true,
                        TalentCategoryId = frontendCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "Miguel Santos",
                        Country = "Portugal",
                        Email = "miguel.santos@devpro.com",
                        HourlyRate = 75.00m,
                        IsPublic = true,
                        TalentCategoryId = backendCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "Sofia Chen",
                        Country = "Portugal",
                        Email = "sofia.chen@clouddev.io",
                        HourlyRate = 85.00m,
                        IsPublic = true,
                        TalentCategoryId = fullstackCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "João Pereira",
                        Country = "Portugal",
                        Email = "joao.pereira@devops.tech",
                        HourlyRate = 90.00m,
                        IsPublic = true,
                        TalentCategoryId = devopsCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "Mariana Silva",
                        Country = "Portugal",
                        Email = "mariana.silva@mobiledev.pt",
                        HourlyRate = 70.00m,
                        IsPublic = true,
                        TalentCategoryId = mobileCategory.Id,
                        UserId = testTalentUser.Id
                    },
                    new Talent
                    {
                        Name = "Carlos Martins",
                        Country = "Portugal",
                        Email = "carlos.martins@fullstack.dev",
                        HourlyRate = 80.00m,
                        IsPublic = true,
                        TalentCategoryId = fullstackCategory.Id,
                        UserId = testTalentUser.Id
                    }
                };

                context.Talents.AddRange(talents);
                await context.SaveChangesAsync();

                // Add skills to talents
                var reactSkill = await context.Skills.FirstAsync(s => s.Name == "React");
                var nextjsSkill = await context.Skills.FirstAsync(s => s.Name == "Next.js");
                var typescriptSkill = await context.Skills.FirstAsync(s => s.Name == "TypeScript");
                var csharpSkill = await context.Skills.FirstAsync(s => s.Name == "C#");
                var aspnetSkill = await context.Skills.FirstAsync(s => s.Name == "ASP.NET Core");
                var nodeSkill = await context.Skills.FirstAsync(s => s.Name == "Node.js");
                var dockerSkill = await context.Skills.FirstAsync(s => s.Name == "Docker");
                var kubernetesSkill = await context.Skills.FirstAsync(s => s.Name == "Kubernetes");
                var reactNativeSkill = await context.Skills.FirstAsync(s => s.Name == "React Native");
                var flutterSkill = await context.Skills.FirstAsync(s => s.Name == "Flutter");

                var talentSkills = new List<TalentSkill>
                {
                    // Ana Rodrigues (Frontend) - React + TypeScript
                    new TalentSkill { TalentId = talents[0].Id, SkillId = reactSkill.Id, YearsOfExperience = 4 },
                    new TalentSkill { TalentId = talents[0].Id, SkillId = typescriptSkill.Id, YearsOfExperience = 3 },
                    new TalentSkill { TalentId = talents[0].Id, SkillId = nextjsSkill.Id, YearsOfExperience = 2 },
                    
                    // Miguel Santos (Backend) - C# + ASP.NET
                    new TalentSkill { TalentId = talents[1].Id, SkillId = csharpSkill.Id, YearsOfExperience = 6 },
                    new TalentSkill { TalentId = talents[1].Id, SkillId = aspnetSkill.Id, YearsOfExperience = 5 },
                    
                    // Sofia Chen (Full Stack) - React + Node.js + TypeScript
                    new TalentSkill { TalentId = talents[2].Id, SkillId = reactSkill.Id, YearsOfExperience = 5 },
                    new TalentSkill { TalentId = talents[2].Id, SkillId = nodeSkill.Id, YearsOfExperience = 4 },
                    new TalentSkill { TalentId = talents[2].Id, SkillId = typescriptSkill.Id, YearsOfExperience = 4 },
                    
                    // João Pereira (DevOps) - Docker + Kubernetes
                    new TalentSkill { TalentId = talents[3].Id, SkillId = dockerSkill.Id, YearsOfExperience = 5 },
                    new TalentSkill { TalentId = talents[3].Id, SkillId = kubernetesSkill.Id, YearsOfExperience = 3 },
                    
                    // Mariana Silva (Mobile) - React Native + Flutter
                    new TalentSkill { TalentId = talents[4].Id, SkillId = reactNativeSkill.Id, YearsOfExperience = 3 },
                    new TalentSkill { TalentId = talents[4].Id, SkillId = flutterSkill.Id, YearsOfExperience = 2 },
                    
                    // Carlos Martins (Full Stack) - More diverse skills
                    new TalentSkill { TalentId = talents[5].Id, SkillId = reactSkill.Id, YearsOfExperience = 4 },
                    new TalentSkill { TalentId = talents[5].Id, SkillId = csharpSkill.Id, YearsOfExperience = 5 },
                    new TalentSkill { TalentId = talents[5].Id, SkillId = dockerSkill.Id, YearsOfExperience = 3 }
                };

                context.TalentSkills.AddRange(talentSkills);
                await context.SaveChangesAsync();
            }

            // Seed job proposals if not exists
            if (!await context.JobProposals.AnyAsync())
            {
                var customer = await context.Customers.FirstAsync();
                var reactSkill = await context.Skills.FirstAsync(s => s.Name == "React");
                var nextjsSkill = await context.Skills.FirstAsync(s => s.Name == "Next.js");
                var csharpSkill = await context.Skills.FirstAsync(s => s.Name == "C#");
                var nodeSkill = await context.Skills.FirstAsync(s => s.Name == "Node.js");
                var dockerSkill = await context.Skills.FirstAsync(s => s.Name == "Docker");
                var typescriptSkill = await context.Skills.FirstAsync(s => s.Name == "TypeScript");
                var reactNativeSkill = await context.Skills.FirstAsync(s => s.Name == "React Native");
                var pythonSkill = await context.Skills.FirstAsync(s => s.Name == "Python");

                var frontendCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Frontend Developer");
                var backendCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Backend Developer");
                var fullstackCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Full Stack Developer");
                var devopsCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "DevOps Engineer");
                var mobileCategory = await context.TalentCategories.FirstAsync(tc => tc.Name == "Mobile Developer");

                var jobProposals = new List<JobProposal>
                {
                    new JobProposal
                    {
                        Name = "E-commerce Platform with Next.js",
                        Description = "Build a modern, high-performance e-commerce platform using Next.js 14 with App Router, TypeScript, and Stripe integration. Includes admin dashboard, product management, and real-time analytics.",
                        TotalHours = 180,
                        SkillId = nextjsSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = frontendCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "Microservices API with .NET 8",
                        Description = "Develop a scalable microservices architecture using .NET 8, Entity Framework Core, and clean architecture principles. Includes authentication, logging, and monitoring.",
                        TotalHours = 240,
                        SkillId = csharpSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = backendCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "Cloud-Native App with Docker & Kubernetes",
                        Description = "Design and implement a cloud-native application using Docker containers, Kubernetes orchestration, CI/CD pipelines, and monitoring with Prometheus and Grafana.",
                        TotalHours = 200,
                        SkillId = dockerSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = devopsCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "Real-time Analytics Dashboard",
                        Description = "Create a comprehensive analytics dashboard with React, TypeScript, WebSockets for real-time data, and D3.js for interactive visualizations. Includes user role management and data export features.",
                        TotalHours = 160,
                        SkillId = typescriptSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = frontendCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "Cross-Platform Mobile App",
                        Description = "Develop a feature-rich mobile application using React Native with TypeScript, offline-first architecture, push notifications, and biometric authentication.",
                        TotalHours = 300,
                        SkillId = reactNativeSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = mobileCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "AI-Powered Data Processing Pipeline",
                        Description = "Build an automated data processing pipeline using Python, FastAPI, machine learning models, and Apache Kafka for real-time data streaming and analysis.",
                        TotalHours = 220,
                        SkillId = pythonSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = backendCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "Full Stack SaaS Platform",
                        Description = "Develop a complete SaaS platform with React frontend, Node.js backend, PostgreSQL database, Stripe billing, multi-tenancy, and comprehensive admin panel.",
                        TotalHours = 400,
                        SkillId = nodeSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = fullstackCategory.Id
                    },
                    new JobProposal
                    {
                        Name = "Progressive Web App (PWA)",
                        Description = "Create a high-performance Progressive Web App using React with TypeScript, service workers for offline functionality, and push notifications. Optimized for mobile and desktop.",
                        TotalHours = 140,
                        SkillId = reactSkill.Id,
                        CustomerId = customer.Id,
                        TalentCategoryId = frontendCategory.Id
                    }
                };

                context.JobProposals.AddRange(jobProposals);
                await context.SaveChangesAsync();
            }
        }
    }
}
