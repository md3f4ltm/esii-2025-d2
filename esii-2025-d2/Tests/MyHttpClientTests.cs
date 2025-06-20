using NUnit.Framework;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SkillsUiTests
{
    [TestFixture]
    public class MyHttpClientTests
    {
        [Test]
        public async Task Should_ReturnExpectedContent_When_HttpRequestIsMocked()
        {
            // Arrange
            var expectedContent = "{\"message\":\"Hello from mock!\"}";
            var mockHandler = new Mock<HttpMessageHandler>();

            // Usando a extensão para configurar o mock
            mockHandler.SetupRequest(HttpMethod.Get, "test-endpoint")
                .ReturnsResponse(new StringContent(expectedContent), HttpStatusCode.OK);

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new System.Uri("https://api.example.com/")
            };

            // Act
            var response = await httpClient.GetAsync("test-endpoint");
            var actualContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(actualContent, Is.EqualTo(expectedContent));
        }
    }
}