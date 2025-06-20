using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;
using Moq.Protected;

namespace SkillsUiTests
{
    public static class HttpMessageHandlerMockExtensions
    {
        public static ISetup<HttpMessageHandler, Task<HttpResponseMessage>> SetupRequest(
            this Mock<HttpMessageHandler> mock,
            HttpMethod method,
            string url)
        {
            return mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri != null &&
                        req.RequestUri.ToString().Contains(url, StringComparison.OrdinalIgnoreCase)),
                    ItExpr.IsAny<CancellationToken>());
        }

        public static void SetupRequestSequence(
            this Mock<HttpMessageHandler> mock,
            HttpMethod method,
            string url,
            params HttpResponseMessage[] responses)
        {
            var sequence = mock.Protected().SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method &&
                    req.RequestUri != null &&
                    req.RequestUri.ToString().Contains(url, StringComparison.OrdinalIgnoreCase)),
                ItExpr.IsAny<CancellationToken>());

            foreach (var response in responses)
            {
                sequence = sequence.ReturnsAsync(response);
            }
        }

        public static IReturnsResult<HttpMessageHandler> ReturnsResponse(
            this ISetup<HttpMessageHandler, Task<HttpResponseMessage>> setup,
            HttpContent content,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = content
            };
            return setup.ReturnsAsync(response);
        }
    }
}
