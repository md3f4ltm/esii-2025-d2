using System;
using System.Linq; // para LINQ
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Bunit;
using esii_2025_d2.Components.Pages;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace SkillsUiTests
{
    [TestFixture]
    public class SkillsPageTests : Bunit.TestContext
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;

        public SkillsPageTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost/")
            };
            Services.AddSingleton(_httpClient);
        }

        [SetUp]
        public void Setup()
        {
            _httpMessageHandlerMock.Reset();
        }

        [Test]
        public void SkillsPage_ShouldLoadAndDisplaySkills()
        {
            var skills = new[]
            {
                new { Id = 1, Name = "C#", Area = "Developer" },
                new { Id = 2, Name = "UI Design", Area = "Design" }
            };

            var json = JsonSerializer.Serialize(skills);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            _httpMessageHandlerMock
                .SetupRequest(HttpMethod.Get, "api/skill")
                .ReturnsResponse(content, HttpStatusCode.OK);

            var cut = Render<Skills>();
            cut.WaitForState(() => cut.Markup.Contains("C#"));

            Assert.That(cut.Markup, Does.Contain("C#"));
            Assert.That(cut.Markup, Does.Contain("Developer"));
            Assert.That(cut.Markup, Does.Contain("UI Design"));
            Assert.That(cut.Markup, Does.Contain("Design"));
        }

        [Test]
        public void SkillsPage_ShouldShowNoSkillsMessage_WhenNoSkillsReturned()
        {
            var emptyJson = "[]";
            var content = new StringContent(emptyJson, System.Text.Encoding.UTF8, "application/json");

            _httpMessageHandlerMock
                .SetupRequest(HttpMethod.Get, "api/skill")
                .ReturnsResponse(content, HttpStatusCode.OK);

            var cut = Render<Skills>();
            cut.WaitForState(() => cut.Markup.Contains("No skills found."));

            Assert.That(cut.Markup, Does.Contain("No skills found."));
        }

        [Test]
        public void SkillsPage_ShouldShowErrorMessage_WhenLoadFails()
        {
            _httpMessageHandlerMock
                .SetupRequest(HttpMethod.Get, "api/skill")
                .Throws(new HttpRequestException("API unavailable"));

            var cut = Render<Skills>();
            cut.WaitForState(() => cut.Markup.Contains("Failed to load skills:"));

            Assert.That(cut.Markup, Does.Contain("Failed to load skills:"));
        }

        [Test]
        public void SkillsPage_ShouldOpenAddModal_WhenCreateButtonClicked()
        {
            var emptyJson = "[]";
            var content = new StringContent(emptyJson, System.Text.Encoding.UTF8, "application/json");
            _httpMessageHandlerMock.SetupRequest(HttpMethod.Get, "api/skill").ReturnsResponse(content, HttpStatusCode.OK);

            var cut = Render<Skills>();
            cut.WaitForState(() => cut.Markup.Contains("Create New Skill"));

            cut.Find("button.btn-primary.mb-3").Click();

            cut.WaitForState(() => cut.Markup.Contains("Add Skill"));

            Assert.That(cut.Markup, Does.Contain("Add Skill"));
        }
        
        [Test]
        public void SkillsPage_ShouldShowDeleteConfirmation_WhenDeleteButtonClicked()
        {
            var skills = new[]
            {
                new { Id = 1, Name = "C#", Area = "Developer" }
            };
            var json = JsonSerializer.Serialize(skills);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            _httpMessageHandlerMock.SetupRequest(HttpMethod.Get, "api/skill").ReturnsResponse(content, HttpStatusCode.OK);

            _httpMessageHandlerMock.SetupRequest(HttpMethod.Get, "api/skill/1/inuse")
                .ReturnsResponse(new StringContent("false"), HttpStatusCode.OK);

            var cut = Render<Skills>();
            cut.WaitForState(() => cut.Markup.Contains("C#"));

            var deleteButtons = cut.FindAll("button.btn-danger");
            var deleteButton = deleteButtons.FirstOrDefault(b => b.TextContent.Trim() == "Delete");
            Assert.That(deleteButton, Is.Not.Null);

            deleteButton.Click();

            cut.WaitForState(() => cut.Markup.Contains("Confirm Delete"));
            Assert.That(cut.Markup, Does.Contain("Confirm Delete"));
            Assert.That(cut.Markup, Does.Contain("C#"));
        }


    }
}

