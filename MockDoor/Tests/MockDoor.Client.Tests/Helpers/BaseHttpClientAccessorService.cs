using System.Net.Http;
using System.Threading.Tasks;
using MockDoor.Client.Services;
using Radzen;

namespace MockDoor.Client.Tests.Helpers;

public class BaseHttpClientAccessorService : BaseHttpClientService
{
    public BaseHttpClientAccessorService(HttpClient client, NotificationService notificationService) : base(client, notificationService)
    {
    }

    public async Task<HttpResponseMessage> SafeGetWrapperAsync(string endpoint, string errorMessage = null, int timeout = 0)
    {
        return await SafeGetAsync(endpoint, errorMessage, timeout);
    }
}