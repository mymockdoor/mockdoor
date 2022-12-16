using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MockDoor.Client.Services;
using MockDoor.Shared.Models.Configuration;
using Radzen;

namespace MockDoor.Client.Shared;

public class CustomErrorBoundary : ErrorBoundary
{
    [Inject]
    private IWebAssemblyHostEnvironment Environment { get; set; }
    
    [Inject]
    private NotificationService NotificationService { get; set; }
    
    [Inject]
    private ConfigurationService ConfigurationService { get; set; }
    
    private DeploymentConfiguration _deploymentConfiguration;
    
    protected override async Task OnInitializedAsync()
    {
        var response = await ConfigurationService.GetDeploymentConfiguration();

        if (response.IsSuccessStatusCode)
        {
            _deploymentConfiguration = response.Content;
        } 
    }

    protected override Task OnErrorAsync(Exception exception)
    {
        if ((_deploymentConfiguration?.Debug ?? false) || Environment.IsDevelopment())
        {
            NotificationService.Notify(NotificationSeverity.Error, exception.GetType().ToString(), exception.Message, duration: 10000);
            return base.OnErrorAsync(exception);
        }
        else
        {
            NotificationService.Notify(NotificationSeverity.Error, "Opps, something went wrong", exception.Message, duration: 10000);
        }
        return Task.CompletedTask;
    }
}