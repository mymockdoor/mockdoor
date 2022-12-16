using MockDoor.Client.Models;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.General;
using Radzen;

namespace MockDoor.Client.Services;

public class MockTestingService : BaseHttpClientService
{
    public MockTestingService(HttpClient client, NotificationService notificationService) : base(client, notificationService)
    {
    }

    public async Task<HttpServiceResult> TestUrlAsync(string requestUrl, RestType requestType, string testBody, List<HeaderItem> headers, string contentType = "application/json", int timeout = 0)
    {
        
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.RequestUri = new Uri(requestUrl, UriKind.Relative);
            
            httpRequestMessage.Headers.Clear();
            foreach (var header in headers)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(header.Name, header.Value);
            }
            switch (requestType)
            {
                case RestType.GET:
                    {
                        httpRequestMessage.Method = HttpMethod.Get;
                    }
                    break;
                case RestType.POST:
                    {
                        httpRequestMessage.Method = HttpMethod.Post;
                        var body = ConvertHelper.ToExactStringContent(testBody, contentType);

                        httpRequestMessage.Content = body;
                    }
                    break;
                case RestType.PUT:
                    {
                        httpRequestMessage.Method = HttpMethod.Put;
                        var body = ConvertHelper.ToExactStringContent(testBody, contentType);

                        httpRequestMessage.Content = body;
                    }
                    break;
                case RestType.PATCH:
                    {
                        httpRequestMessage.Method = HttpMethod.Patch;
                        var body = ConvertHelper.ToExactStringContent(testBody, contentType);

                        httpRequestMessage.Content = body;
                    }
                    break;
                case RestType.DELETE:
                    {
                        httpRequestMessage.Method = HttpMethod.Delete;
                }
                    break;
            }
        var response = await SafeSendAsync(httpRequestMessage, null, timeout); 
        
        return await HandleResponseAsync(response);
    }
}