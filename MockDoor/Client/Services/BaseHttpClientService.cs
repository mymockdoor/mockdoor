using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.JsonPatch;
using MockDoor.Client.Helpers;
using MockDoor.Client.Models;
using MockDoor.Shared.Models.Utility;
using Radzen;

namespace MockDoor.Client.Services;

public class BaseHttpClientService
{
    protected readonly HttpClient Client;

    private readonly NotificationService _notificationService;

    protected BaseHttpClientService(HttpClient client, NotificationService notificationService)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }
    
    protected async Task<HttpResponseMessage> SafeGetAsync(string endpoint, string errorMessage = null, int timeout = 0)
    {
        HttpResponseMessage response;
        try
        {
            if (timeout > 0)
            {
                using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                response = await Client.GetAsync(endpoint, timeoutToken.Token);
            }
            else
            {
                response = await Client.GetAsync(endpoint);
            }
        }
        catch (TaskCanceledException ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Timed out")
            };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };
        }

        return response;
    }

    protected async Task<HttpResponseMessage> SafeDeleteAsync(string endpoint, string errorMessage, int timeout = 0)
    {
        HttpResponseMessage response;
        try
        {
            if (timeout > 0)
            {
                using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                response = await Client.DeleteAsync(endpoint, timeoutToken.Token);
            }
            else
            {
                response = await Client.DeleteAsync(endpoint);
            }
        }
        catch (TaskCanceledException ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Timed out")
            };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };
        }

        return response;
    }

    protected async Task<HttpResponseMessage> SafePostAsync(string endpoint, string errorMessage, int timeout = 0)
    {
        HttpResponseMessage response;
        try
        {
            if (timeout > 0)
            {
                using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                response = await Client.PostAsync(endpoint, null, timeoutToken.Token);
            }
            else
            {
                response = await Client.PostAsync(endpoint, null);
            }
        }
        catch (TaskCanceledException ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Timed out")
            };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };
        }

        return response;
    }

    protected async Task<HttpResponseMessage> SafePostAsync<T>(string endpoint, T content, string errorMessage, int timeout = 0)
    {
        HttpResponseMessage response;
        try
        {
            if (timeout > 0)
            {
                using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                response = await Client.PostAsJsonAsync(endpoint, content, timeoutToken.Token);
            }
            else
            {
                response = await Client.PostAsJsonAsync(endpoint, content);
            }
        }
        catch (TaskCanceledException ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Timed out")
            };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };
        }

        return response;
    }

    protected async Task<HttpResponseMessage> SafePostAsync(string endpoint, string content, string errorMessage, int timeout = 0)
    {
        HttpResponseMessage response;
        try
        {
            if (timeout > 0)
            {
                using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                response = await Client.PostAsync(endpoint, new StringContent(content), timeoutToken.Token);
            }
            else
            {
                response = await Client.PostAsync(endpoint, new StringContent(content));
            }
        }
        catch (TaskCanceledException ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Timed out")
            };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };
        }

        return response;
    }

    protected async Task<HttpResponseMessage> SafePatchAsync<T>(string endpoint, JsonPatchDocument<T> content, string errorMessage, int timeout = 0) where T : class
    {
        HttpResponseMessage response;
        try
        {
            if (timeout > 0)
            {
                using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                response = await Client.PatchAsync(endpoint, content, timeoutToken);
            }
            else
            {
                response = await Client.PatchAsync(endpoint, content);
            }
        }
        catch (TaskCanceledException ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Timed out")
            };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };
        }

        return response;
    }

    protected async Task<HttpResponseMessage> SafePutAsync<T>(string endpoint, T content, string errorMessage, int timeout = 0)
    {
        HttpResponseMessage response;
        try
        {
            if (timeout > 0)
            {
                using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                response = await Client.PutAsJsonAsync(endpoint, content, timeoutToken.Token);
            }
            else
            {
                response = await Client.PutAsJsonAsync(endpoint, content);
            }
        }
        catch (TaskCanceledException ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Timed out")
            };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };
        }

        return response;
    }
    
    protected async Task<HttpResponseMessage> SafeSendAsync(HttpRequestMessage httpRequestMessage, string errorMessage, int timeout = 0)
    {
        HttpResponseMessage response;
        try
        {
            if (timeout > 0)
            {
                using var timeoutToken = new  CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                response = await Client.SendAsync(httpRequestMessage, timeoutToken.Token);
            }
            else
            {
                response = await Client.SendAsync(httpRequestMessage);
            }
        }
        catch (TaskCanceledException ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Timed out")
            };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (errorMessage.Contains("{0}"))
                {
                    NotifyError(string.Format(errorMessage, ex.Message));
                }
                else
                {
                    NotifyError(errorMessage);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message)
            };
        }

        return response;
    }
    
    protected async Task<HttpServiceResult<string>> HandleResponseAsStringAsync(HttpResponseMessage response, string errorMessage = null, string successMessage = null, bool apiSupportsBadResponseDto = false)
    {
        var result = new HttpServiceResult<string>()
        {
            OriginalResponse = response
        };

        if (!response.IsSuccessStatusCode)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                NotifyError(errorMessage);
                result.Message = errorMessage;
            }

           
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (apiSupportsBadResponseDto)
                {
                    try
                    {
                        result.BadRequestResult = await response.GetContentAsync<BadRequestResultDto>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured parsing bad request. {errorMessage}", ex.Message);
                        NotifyError("An errored occured parsing bad request");
                    }
                }
                else
                {
                    try
                    {
                        result.Message = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured parsing bad request. {errorMessage}", ex.Message);
                        NotifyError("An errored occured parsing bad request");
                    }
                }
            }
        }
        else
        {
            try
            {
                result.Content = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured parsing bad request. {errorMessage}", ex.Message);
                NotifyError("Error occured reading this request");
            }

            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                NotifySuccess(successMessage);
                result.Message = successMessage;
            }
        }

        return result;
    }
    
    protected async Task<HttpServiceResult<T>> HandleResponseAsync<T>(HttpResponseMessage response, string errorMessage = null, string successMessage = null, bool apiSupportsBadResponseDto = false) where T : class
    {
        var result = new HttpServiceResult<T>()
        {
            OriginalResponse = response
        };

        if (!response.IsSuccessStatusCode)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                NotifyError(errorMessage);
                result.Message = errorMessage;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (apiSupportsBadResponseDto)
                {
                    try
                    {
                        result.BadRequestResult = await response.GetContentAsync<BadRequestResultDto>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured parsing bad request. {errorMessage}", ex.Message);
                        NotifyError("An errored occured parsing bad request");
                    }
                }
                else
                {
                    try
                    {
                        result.Message = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured parsing bad request. {errorMessage}", ex.Message);
                        NotifyError("An errored occured parsing bad request");
                    }
                }
            }
        }
        else
        {
            result.Content = await response.GetContentAsync<T>();
            
            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                NotifySuccess(successMessage);
                result.Message = successMessage;
            }
        }

        return result;
    }

    protected async Task<HttpServiceResult> HandleResponseAsync(HttpResponseMessage response, string errorMessage = null, string successMessage = null, bool apiSupportsBadResponseDto = false)
    {
        var result = new HttpServiceResult()
        {
            OriginalResponse = response
        };

        if (!response.IsSuccessStatusCode)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                NotifyError(errorMessage);
                result.Message = errorMessage;
            }

            if (apiSupportsBadResponseDto && response.StatusCode == HttpStatusCode.BadRequest)
            {
                try
                {
                    result.BadRequestResult = await response.GetContentAsync<BadRequestResultDto>();
                }
                catch (Exception)
                {
                    NotifyError("An errored occured parsing bad request");
                }
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                NotifySuccess(successMessage);
                result.Message = successMessage;
            }
        }

        return result;
    }

    protected void NotifySuccess(string message)
    {
        _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Success", Detail = message, Duration = 4000 });        
    }

    protected void NotifyError(string message)
    {
        _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = message, Duration = 4000 });        
    }

    protected void NotifyWarning(string message)
    {
        _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Warning, Summary = "Warning", Detail = message, Duration = 10000 });        
    }
}