using MockDoor.Shared.Models.Utility;

namespace MockDoor.Client.Models;

public class HttpServiceResult<T> : HttpServiceResult
{
    public T Content { get; set; }
}

public class HttpServiceResult
{
    public HttpResponseMessage OriginalResponse { get; set; }

    public BadRequestResultDto BadRequestResult { get; set; }

    public string Message { get; set; }

    public bool IsSuccessStatusCode => OriginalResponse?.IsSuccessStatusCode ?? false;
}