using System.Net.Http.Json;
using MockDoor.Client.Helpers;
using MockDoor.Client.Models;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.ServiceGroup;
using MockDoor.Shared.Models.Tenant;
using Radzen;

namespace MockDoor.Client.Services;

public class SetupWizardService : BaseHttpClientService
{
    public SetupWizardService(HttpClient httpClient, NotificationService notificationService) : base(httpClient, notificationService)
    { }

    internal async Task<TenantDeploymentItem> DeployTenantAsync(TenantDeploymentItem tenantDeploymentItem)
    {
        var response = await Client.PostAsJsonAsync("api/tenant", tenantDeploymentItem.Tenant);

        if (response.IsSuccessStatusCode)
        {
            tenantDeploymentItem.Description = "Deployed Successfully";
            tenantDeploymentItem.DeploymentStatus = DeploymentStatus.Complete;
            tenantDeploymentItem.Tenant = await response.Content.GetContentAsync<BaseTenantDto>();
        }
        else
        {
            tenantDeploymentItem.Description = "Deployment failed";
            tenantDeploymentItem.FailureMessage = response.StatusCode.ToString();
            tenantDeploymentItem.DeploymentStatus = DeploymentStatus.Failed;
            NotifyError($"Deployment failed for tenant {tenantDeploymentItem.Tenant.Name}");
        }

        return tenantDeploymentItem;
    }

    internal async Task<TenantDeploymentItem> CheckExistingTenantAsync(TenantDeploymentItem tenantDeploymentItem)
    {
        var response = await Client.GetAsync($"api/tenant/{tenantDeploymentItem.Tenant.Id}");

        if (response.IsSuccessStatusCode)
        {
            tenantDeploymentItem.Description = "Tenant found";
            tenantDeploymentItem.DeploymentStatus = DeploymentStatus.Complete;
            tenantDeploymentItem.Tenant = await response.Content.GetContentAsync<BaseTenantDto>();
        }
        else
        {
            tenantDeploymentItem.Description = "Tenant not found";
            tenantDeploymentItem.FailureMessage = response.StatusCode.ToString();
            tenantDeploymentItem.DeploymentStatus = DeploymentStatus.Failed;
            NotifyError($"Deployment failed for existing tenant {tenantDeploymentItem.Tenant.Name}, it was not found");
        }

        return tenantDeploymentItem;
    }

    internal async Task<ServiceGroupDeploymentItem> DeployServiceGroupAsync(int tenantId,
        ServiceGroupDeploymentItem serviceGroupDeploymentItem)
    {
        var existsCheckResponse =
            await Client.GetAsync($"api/servicegroup/findbypath/{tenantId}/{serviceGroupDeploymentItem.ServiceGroup.Path}");

        if (existsCheckResponse.IsSuccessStatusCode)
        {
            serviceGroupDeploymentItem.Description = "Group already exists";
            serviceGroupDeploymentItem.DeploymentStatus = DeploymentStatus.AlreadyExists;
            serviceGroupDeploymentItem.ServiceGroup = await existsCheckResponse.Content.GetContentAsync<BasicServiceGroupDto>();
        }
        else
        {
            var response =
                await Client.PostAsJsonAsync($"api/servicegroup/{tenantId}",
                    serviceGroupDeploymentItem.ServiceGroup);

            if (response.IsSuccessStatusCode)
            {
                serviceGroupDeploymentItem.Description = "Deployed Successfully";
                serviceGroupDeploymentItem.DeploymentStatus = DeploymentStatus.Complete;
                serviceGroupDeploymentItem.ServiceGroup = await response.Content.GetContentAsync<BasicServiceGroupDto>();
            }
            else
            {
                serviceGroupDeploymentItem.Description = $"Failed to deploy";
                serviceGroupDeploymentItem.FailureMessage = response.StatusCode.ToString();
                serviceGroupDeploymentItem.DeploymentStatus = DeploymentStatus.Failed;
                NotifyError($"Deployment failed for service group {serviceGroupDeploymentItem.ServiceGroup.Name}");
            }
        }

        return serviceGroupDeploymentItem;
    }

    internal async Task<MicroserviceDeploymentItem> DeployMicroserviceAsync(int serviceGroupId,
        MicroserviceDeploymentItem microserviceDeploymentItem)
    {
        var response = await Client.PostAsJsonAsync("api/microservice/" + serviceGroupId,
            microserviceDeploymentItem.Microservice);

        if (response.IsSuccessStatusCode)
        {
            microserviceDeploymentItem.Description = "Deployed Successfully";
            microserviceDeploymentItem.DeploymentStatus = DeploymentStatus.Complete;
            microserviceDeploymentItem.Microservice = await response.Content.GetContentAsync<MicroserviceResultDto>();
        }
        else
        {
            microserviceDeploymentItem.Description = $"Failed to deploy";
            microserviceDeploymentItem.FailureMessage = await response.Content.ReadAsStringAsync();
            microserviceDeploymentItem.DeploymentStatus = DeploymentStatus.Failed;
            NotifyError($"Deployment failed for microservice group {microserviceDeploymentItem.Microservice.Name}");
        }

        return microserviceDeploymentItem;
    }

    public void NotifyComplete()
    {
        NotifySuccess("All deployments completed successfully");
    }

    public void NotifyWarningMicroserviceExists(string microserviceName)
    {
        NotifyWarning($"{microserviceName} already existed so was ignored and the any changes were not applied!");
    }
}