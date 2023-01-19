using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MockDoor.Client.Constants;
using MockDoor.Client.Services;
using MockDoor.Client.State;
using MockDoor.Shared.Models.Configuration;
using MockDoor.Shared.Models.Utility;
using Radzen;

namespace MockDoor.Client.Shared
{
    public partial class MainLayout
    {
        string _connectionString;

        string _status;
        bool _testing;
        ConnectionStringStatus _connectionStatus = ConnectionStringStatus.Untested;
        
        [Inject]
        private IWebAssemblyHostEnvironment Environment { get; set; }
        
        [Inject] private ConfigurationService ConfigurationService { get; set; }
        
        [Inject]
        private PageHistoryState PageHistoryState { get; set; }
        
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        
        private CustomErrorBoundary _errorBoundary;

        private DeploymentConfiguration _deploymentConfiguration;
        private string _errorsOnLoad;
        private BadRequestResultDto _badRequestResult;

        protected override void OnInitialized()
        {
            _errorBoundary = new CustomErrorBoundary();
            if (!PageHistoryState.CanGoBack())
            {
                PageHistoryState.AddPageToHistory(NavigationManager.Uri);
            }
        }

        protected override async Task OnInitializedAsync()
        { 
            var response = await ConfigurationService.GetDeploymentConfiguration();

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                _deploymentConfiguration = response.Content;
                _connectionString = _deploymentConfiguration?.DatabaseConfig?.ConnectionString ?? "not found";
            }   
            else if(!string.IsNullOrWhiteSpace(response.Message))
            {
                _errorsOnLoad = response.Message;
            }
            else
            {
                _errorsOnLoad = UiConstants.UnknownErrorOccured;
            }
        }

        async Task OnSubmitNewConnectionString()
        {
            _testing = true;
            StateHasChanged();

            await Task.Delay(1);

            
            var result = await ConfigurationService.TestConnection(_connectionString);

            if (result.IsSuccessStatusCode)
            {
                var content = result.Content;
                _connectionStatus = content.ConnectionStringStatus;
                _status = result.Content?.Message;
            }
            else
            {
                _status = result.Message;
                _connectionStatus = ConnectionStringStatus.Failed;
            }
            _testing = false;
            StateHasChanged();
        }

        async Task ApplyMigrations()
        {
            _testing = true;
            StateHasChanged();

            await Task.Delay(1);

            var result = await ConfigurationService.ApplyMigrationsAsync();

            if (result.IsSuccessStatusCode)
            {
                _status = "Success!";
                _deploymentConfiguration.PendingMigrations = new List<string>();
                _deploymentConfiguration.SqlConnectionStatus = ConnectionStringStatus.Success;
            }
            else
            {
                _badRequestResult = result.BadRequestResult;
            }
            _testing = false;
            StateHasChanged();
        }

        List<string> GetPendingMigrations()
        {
            return _deploymentConfiguration?.PendingMigrations?.ToList() ?? new List<string>();
        }

        private AlertStyle StatusToAlertStyle()
        {
            switch (_connectionStatus)
            {
                case ConnectionStringStatus.ConnectNoDatabase:
                case ConnectionStringStatus.Success: return AlertStyle.Success;
                case ConnectionStringStatus.Failed: return AlertStyle.Danger;
                case ConnectionStringStatus.Untested: return AlertStyle.Light;
                default: return AlertStyle.Primary;
            }
        }
    }
}
