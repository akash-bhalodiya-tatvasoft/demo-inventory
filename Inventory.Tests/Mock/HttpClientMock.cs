using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;

namespace Inventory.Tests.Mock;

public static class HttpClientMock
{
    public static HttpClient Create(object responseObject, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new Mock<HttpMessageHandler>();

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(
                    JsonSerializer.Serialize(responseObject),
                    Encoding.UTF8,
                    "application/json")
            });

        return new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("https://fakeapi.com/")
        };
    }
}
