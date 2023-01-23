using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MockDoor.Client.Services;
using MockDoor.Client.Tests.Helpers;
using Moq;
using Radzen;
using RichardSzalay.MockHttp;
using Xunit;

namespace MockDoor.Client.Tests.ServicesTests;

public class BaseHttpClientServiceTests
{
    private readonly MockHttpMessageHandler _handlerMock = new ();
    
    [Fact]
    public async Task SafeGetAsync_Timeout_ReturnsBadRequest()
    {
        // Arrange
        var timeout = 1;
        var errorMessage = "Timed out";
        var endpoint = "https://example.com";
        var mockClient =_handlerMock.ToHttpClient();

        _handlerMock.When(HttpMethod.Get, endpoint)
            .Throw(new TaskCanceledException());
        
        var mockNotificationService = new Mock<NotificationService>();
        var service = new BaseHttpClientAccessorService(mockClient, mockNotificationService.Object);

        // Act
        var response = await service.SafeGetWrapperAsync(endpoint, errorMessage, timeout);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        
        Assert.Equal(errorMessage, content);
    }
}