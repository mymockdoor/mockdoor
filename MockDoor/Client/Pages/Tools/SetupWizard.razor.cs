using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MockDoor.Client.Models;
using MockDoor.Client.Services;
using MockDoor.Shared;
using MockDoor.Shared.Models.Headers;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.ServiceGroup;
using MockDoor.Shared.Models.Tenant;
using Radzen;
using Radzen.Blazor;

namespace MockDoor.Client.Pages.Tools;

public partial class SetupWizard
{
    private readonly MicroserviceResultDto _defaultMicroservice = new();

    private TenantListDto _existingTenants;

    private bool _hasConfirmedDefaults, _deploying;
    private RadzenDataGrid<SnapshotEntity<MicroserviceResultDto>> _microservicesGrid;

    private string _newTenantPathError, _newTenantNameError, _newMicroserviceNameError, _newMicroservicePathError;
    private bool _newTenantNameValid, _newTenantPathValid;
    
    private BaseServiceGroupDto _defaultGroupDetails = new ()
    {
        Name = "Default",
        Path =  "default"
    }; 
    
    private bool _groupNameIsValid = true;
    private bool _groupPathIsValid = true;
    private bool _groupUrlIsValid = true;

    private RadzenDataGrid<SnapshotEntity<BaseTenantDto>> _tenantsGrid;

    private RadzenDataGrid<BaseTenantDto> _existingTenantsGrid;

    [Inject] private DialogService DialogService { get; set; }

    [Inject] private TenantService TenantService { get; set; }

    [Inject] private SetupWizardService SetupWizardService { get; set; }

    private SnapshotEntity<BaseTenantDto> NewTenant { get; set; } = new(new BaseTenantDto());
    
    private SnapshotEntity<MicroserviceResultDto> NewMicroservice { get; set; } = new(new MicroserviceResultDto());

    private List<SnapshotEntity<BaseTenantDto>> TenantCollection { get; } = new();
    
    private List<BaseTenantDto> SelectedExistingTenants { get; } = new();

    private int? _selectedExistingTenant;

    private List<SnapshotEntity<MicroserviceResultDto>> MicroserviceCollection { get; } = new();

    private IList<SnapshotEntity<MicroserviceResultDto>> SelectedMicroservice { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await TenantService.GetTenantListAsync();

        if (response.IsSuccessStatusCode)
        {
            _existingTenants = response.Content;
        }

        await base.OnInitializedAsync();
    }

    #region Deployment

    private readonly List<TenantDeploymentItem> _deploymentItems = new();
    private bool _newMicroserviceNameValid, _newMicroservicePathValid, _newMicroserviceUrlValid;

    private async Task DeployAsync()
    {
        _deploying = true;
        foreach (var tenantEntity in TenantCollection)
        {
            var tenantDeploymentItem = new TenantDeploymentItem
            {
                Tenant = tenantEntity.Value,
                Description = $"{tenantEntity.Value.Name}",
                DeploymentStatus = DeploymentStatus.NotStarted,
                ServiceGroupDeploymentItem = new ServiceGroupDeploymentItem
                {
                    Description = _defaultGroupDetails.Name,
                    DeploymentStatus = DeploymentStatus.NotStarted,
                    ServiceGroup = new BasicServiceGroupDto
                    {
                        Enabled = true, Name = _defaultGroupDetails.Name, Path = _defaultGroupDetails.Path, DefaultHealthCheckUrl = _defaultGroupDetails.DefaultHealthCheckUrl, Microservices = new ()
                    }
                }
            };

            foreach (var microserviceEntity in MicroserviceCollection)
                tenantDeploymentItem.MicroserviceDeployments.Add(new MicroserviceDeploymentItem
                {
                    Description = microserviceEntity.Value.Name,
                    Microservice = microserviceEntity.Value,
                    DeploymentStatus = DeploymentStatus.NotStarted
                });
            _deploymentItems.Add(tenantDeploymentItem);
        }
        
        foreach (var existingTenant in SelectedExistingTenants)
        {
            var tenantDeploymentItem = new TenantDeploymentItem
            {
                Tenant = existingTenant,
                Description = $"{existingTenant.Name}",
                DeploymentStatus = DeploymentStatus.NotStarted,
                ServiceGroupDeploymentItem = new ServiceGroupDeploymentItem
                {
                    Description = _defaultGroupDetails.Name,
                    DeploymentStatus = DeploymentStatus.NotStarted,
                    ServiceGroup = new BasicServiceGroupDto
                    {
                        Enabled = true, Name = _defaultGroupDetails.Name, Path = _defaultGroupDetails.Path, DefaultHealthCheckUrl = _defaultGroupDetails.DefaultHealthCheckUrl, Microservices = new()
                    }
                }
            };

            foreach (var microserviceEntity in MicroserviceCollection)
                tenantDeploymentItem.MicroserviceDeployments.Add(new MicroserviceDeploymentItem
                {
                    Description = microserviceEntity.Value.Name,
                    Microservice = microserviceEntity.Value,
                    DeploymentStatus = DeploymentStatus.NotStarted
                });
            _deploymentItems.Add(tenantDeploymentItem);
        }

        StateHasChanged();
        foreach (var deploymentItem in _deploymentItems)
        {
            await Task.Run(async () =>
            {
                deploymentItem.DeploymentStatus = DeploymentStatus.InProgress;
                StateHasChanged();

                TenantDeploymentItem tenantResponse;
                if (deploymentItem.Tenant.Id > 0)
                {
                    tenantResponse = await SetupWizardService.CheckExistingTenantAsync(deploymentItem);
                }
                else
                {
                    tenantResponse = await SetupWizardService.DeployTenantAsync(deploymentItem);
                }

                deploymentItem.DeploymentStatus = tenantResponse.DeploymentStatus;

                if (tenantResponse.DeploymentStatus == DeploymentStatus.Complete)
                {
                    tenantResponse.ServiceGroupDeploymentItem.ServiceGroup.TenantId = tenantResponse.Tenant.Id;
                    var groupResponse = await SetupWizardService.DeployServiceGroupAsync(tenantResponse.Tenant.Id,
                        tenantResponse.ServiceGroupDeploymentItem);

                    deploymentItem.ServiceGroupDeploymentItem.DeploymentStatus = groupResponse.DeploymentStatus;

                    foreach (var microserviceDeploymentItem in deploymentItem.MicroserviceDeployments)
                    {
                        if (groupResponse.DeploymentStatus == DeploymentStatus.Complete ||
                            groupResponse.DeploymentStatus == DeploymentStatus.AlreadyExists)
                        {
                            if (groupResponse.DeploymentStatus == DeploymentStatus.AlreadyExists &&
                                groupResponse.ServiceGroup.Microservices.Any(ms =>
                                    string.Equals(ms.Path, microserviceDeploymentItem.Microservice.Path,
                                        StringComparison.OrdinalIgnoreCase)))
                            {
                                microserviceDeploymentItem.DeploymentStatus = DeploymentStatus.AlreadyExists;
                                SetupWizardService.NotifyWarningMicroserviceExists(microserviceDeploymentItem.Microservice.Name);
                            }
                            else
                            {
                                await DeployMicroservicesAsync(microserviceDeploymentItem, groupResponse);
                            }
                        }
                        else
                        {
                            microserviceDeploymentItem.DeploymentStatus = DeploymentStatus.Failed;
                        }
                    }
                }
                else
                {
                    deploymentItem.ServiceGroupDeploymentItem.DeploymentStatus = DeploymentStatus.Failed;
                    foreach (var microserviceDeploymentItem in deploymentItem.MicroserviceDeployments)
                        microserviceDeploymentItem.DeploymentStatus = DeploymentStatus.Failed;
                }
                StateHasChanged();
            });
        }

        if (GetFullDeploymentStatus() == DeploymentStatus.Complete)
        {
            SetupWizardService.NotifyComplete();
        }
    }

    private async Task DeployMicroservicesAsync(MicroserviceDeploymentItem microserviceDeploymentItem,
        ServiceGroupDeploymentItem response)
    {
        await Task.Run(async () =>
        {
            microserviceDeploymentItem.Microservice.RegisteredServiceGroupId =
                response.ServiceGroup.Id;
            microserviceDeploymentItem.DeploymentStatus = DeploymentStatus.InProgress;
            StateHasChanged();
            var microserviceResponse = await SetupWizardService.DeployMicroserviceAsync(response.ServiceGroup.Id,
                microserviceDeploymentItem);

            microserviceDeploymentItem.DeploymentStatus = microserviceResponse.DeploymentStatus;
            StateHasChanged();
        });
    }

    private DeploymentStatus GetFullDeploymentStatus()
    {
        if (_deploymentItems.All(d => d.DeploymentStatus == DeploymentStatus.NotStarted))
            return DeploymentStatus.NotStarted;

        var allDeploymentStatuses =
            _deploymentItems.SelectMany(d => d.MicroserviceDeployments).Select(m => m.DeploymentStatus)
                .Union(_deploymentItems.Select(d => d.DeploymentStatus)).ToList();

        if (allDeploymentStatuses.Any(d => d == DeploymentStatus.NotStarted || d == DeploymentStatus.InProgress))
            return DeploymentStatus.InProgress;

        if (allDeploymentStatuses.Any(d => d == DeploymentStatus.Failed)) return DeploymentStatus.Failed;

        return DeploymentStatus.Complete;
    }

    #endregion

    #region Microservice

    private async Task EnterOnMicroserviceAsync(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
            if (ValidateMicroservice())
                await InsertMicroserviceAsync();
    }

    private void ConfirmDefaults()
    {
        var newMicroservice = _defaultMicroservice.CopyTo(new MicroserviceResultDto());
        NewMicroservice = new SnapshotEntity<MicroserviceResultDto>(newMicroservice);
        _hasConfirmedDefaults = !_hasConfirmedDefaults;
    }
    
    private bool ValidateMicroservice()
    {
        _newMicroserviceNameError = null;
        _newMicroservicePathError = null;
        if (NewMicroservice?.Value == null || MicroserviceCollection == null || _defaultMicroservice == null) return false;

        _newMicroserviceNameError = GetMicroserviceNameError(NewMicroservice);
        _newMicroservicePathError = GetMicroservicePathError(NewMicroservice);

        return string.IsNullOrWhiteSpace(_newMicroserviceNameError) && _newMicroserviceNameValid &&
               string.IsNullOrWhiteSpace(_newMicroservicePathError) && _newMicroservicePathValid &&
               _newMicroserviceUrlValid;
    }
    
    private async Task InsertMicroserviceAsync()
    {
        await _microservicesGrid.InsertRow(NewMicroservice);
        MicroserviceCollection.Add(NewMicroservice);
        await SaveMicroserviceRowAsync(NewMicroservice);
        
        var newMicroservice = _defaultMicroservice.CopyTo(new MicroserviceResultDto());
        NewMicroservice = new SnapshotEntity<MicroserviceResultDto>(newMicroservice);
        _newMicroserviceNameValid = false;
        _newMicroservicePathValid = false;
        _newMicroserviceUrlValid = false;
        StateHasChanged();
    }

    private async Task EditMicroserviceRowAsync(SnapshotEntity<MicroserviceResultDto> microservice)
    {
        await _microservicesGrid.EditRow(microservice);
    }

    private void OnUpdateMicroserviceRow(SnapshotEntity<MicroserviceResultDto> microservice)
    {
        microservice.CommitChanges();
    }

    private async Task SaveMicroserviceRowAsync(SnapshotEntity<MicroserviceResultDto> microservice)
    {
        microservice.CommitChanges();
        await _microservicesGrid.UpdateRow(microservice);
    }

    private void CancelMicroserviceEdit(SnapshotEntity<MicroserviceResultDto> microservice)
    {
        microservice.RestoreValue();
        _microservicesGrid.CancelEditRow(microservice);
    }

    private async Task ConfirmDeleteMicroserviceRowAsync(SnapshotEntity<MicroserviceResultDto> microservice)
    {
        var confirmed = await DialogService.Confirm("Are you sure?", "Confirm Delete?",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != null && confirmed.Value)
        {
            if (_microservicesGrid.IsRowInEditMode(microservice)) CancelMicroserviceEdit(microservice);

            MicroserviceCollection.Remove(microservice);

            await _microservicesGrid.Reload();
        }
    }

    private bool IsMicroserviceValid(SnapshotEntity<MicroserviceResultDto> microservice)
    {
        if (string.IsNullOrWhiteSpace(microservice.Value.Name) ||
            string.IsNullOrWhiteSpace(microservice.Value.TargetUrl) ||
            string.IsNullOrWhiteSpace(microservice.Value.Path) ||
            microservice.Value.IsValid(
                new Dictionary<object, object>()
                {
                    { "Name", MicroserviceCollection.Select(t => t.Value.Name) },
                    { "Path", MicroserviceCollection.Select(t => t.Value.Path) }
                }))
            return false;

        return true;
    }

    private void CycleHeaderMode(ServiceHeaderDto header)
    {
        if (header.Incoming && header.Outgoing)
        {
            header.Outgoing = false;
        }
        else if (header.Incoming)
        {
            header.Incoming = false;
            header.Outgoing = true;
        }
        else
        {
            header.Incoming = true;
        }
    }

    private static string GetHeaderButtonDescription(ServiceHeaderDto header)
    {
        if (header.Incoming && header.Outgoing)
        {
            return "<-->";
        }
        else if (header.Incoming)
        {
            return "<--|";
        }

        return "|-->";
    }

    private string GetMicroserviceNameError(SnapshotEntity<MicroserviceResultDto> microservice)
    {
        if (string.IsNullOrWhiteSpace(microservice.Value.Name))
            return string.Empty; // handled by validator input
        
        if (MicroserviceCollection.Any(m =>
                m.Value.Name.ToUpper() == microservice.Value.Name.ToUpper() && microservice != m))
            return "Name already taken choose another";

        return string.Empty;
    }

    private string GetMicroservicePathError(SnapshotEntity<MicroserviceResultDto> microservice)
    {
        if (string.IsNullOrWhiteSpace(microservice.Value.Path))
            return string.Empty; //handled by validator input

        if (MicroserviceCollection.Any(m =>
                m.Value.Path.ToUpper() == microservice.Value.Path.ToUpper() && microservice != m))
            return "Path already taken choose another";

        return string.Empty;
    }
    #endregion

    #region Tenant

    private async Task EnterOnTenantAsync(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
            if (IsNewTenantValid())
                await InsertTenantAsync();
    }

    private async Task InsertTenantAsync()
    {
        await _tenantsGrid.InsertRow(NewTenant);
        await SaveTenantRowAsync(NewTenant);
        NewTenant = new SnapshotEntity<BaseTenantDto>(new BaseTenantDto());
    }
    
    private async Task InsertExistingTenantAsync()
    {
        var selectedTenant = _existingTenants.Tenants.FirstOrDefault(et => et.Id == _selectedExistingTenant);
        if (_selectedExistingTenant != null)
        {
            await _existingTenantsGrid.InsertRow(selectedTenant);
            SelectedExistingTenants.Add(selectedTenant);
            _selectedExistingTenant = null;
        }
    }

    private void OnCreateTenantRow(SnapshotEntity<BaseTenantDto> tenant)
    {
        TenantCollection.Add(tenant);
    }

    private async Task EditTenantRowAsync(SnapshotEntity<BaseTenantDto> tenant)
    {
        await _tenantsGrid.EditRow(tenant);
    }

    private void OnUpdateTenantRow(SnapshotEntity<BaseTenantDto> tenant)
    {
        tenant.CommitChanges();
    }

    private async Task SaveTenantRowAsync(SnapshotEntity<BaseTenantDto> tenant)
    {
        tenant.CommitChanges();
        await _tenantsGrid.UpdateRow(tenant);
    }

    private void CancelTenantEdit(SnapshotEntity<BaseTenantDto> tenant)
    {
        tenant.RestoreValue();
        _tenantsGrid.CancelEditRow(tenant);
    }

    private async Task ConfirmDeleteTenantRowAsync(SnapshotEntity<BaseTenantDto> tenant)
    {
        var confirmed = await DialogService.Confirm("Are you sure?", "Confirm Delete?",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != null && confirmed.Value)
        {
            if (_tenantsGrid.IsRowInEditMode(tenant)) CancelTenantEdit(tenant);

            TenantCollection.Remove(tenant);

            await _tenantsGrid.Reload();
        }
    }

    private async Task ConfirmRemoveExistingTenantRowAsync(BaseTenantDto tenant)
    {
        var confirmed = await DialogService.Confirm("Are you sure?", "Confirm remove?",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != null && confirmed.Value)
        {
            SelectedExistingTenants.Remove(tenant);

            await _existingTenantsGrid.Reload();
        }
    }

    private bool IsNewTenantValid()
    {
        _newTenantNameError = null;
        _newTenantPathError = null;
        if (NewTenant?.Value == null || TenantCollection == null || _existingTenants == null) return false;

        _newTenantNameError = GetTenantNameError(NewTenant);
        _newTenantPathError = GetTenantPathError(NewTenant);

        return string.IsNullOrWhiteSpace(_newTenantNameError) && _newTenantNameValid && string.IsNullOrWhiteSpace(_newTenantPathError) && _newTenantPathValid;
    }

    private bool IsTenantValid(SnapshotEntity<BaseTenantDto> tenant)
    {
        return IsTenantNameValid(tenant) && 
               IsTenantPathValid(tenant) && 
               tenant.Value.IsValid(
                   new Dictionary<object, object>()
                        {
                            { "Path", _existingTenants.Tenants.Select(t => t.Path) },
                            { "Name", _existingTenants.Tenants.Select(t => t.Name) }
                        });
    }

    private bool IsTenantNameValid(SnapshotEntity<BaseTenantDto> tenant)
    {
        var name = tenant?.Value?.Name?.ToUpper();
        if (TenantCollection == null || _existingTenants == null) return false;

         if (string.IsNullOrWhiteSpace(name))
            return false;

         return true;
    }

    private bool IsTenantPathValid(SnapshotEntity<BaseTenantDto> tenant)
    {
        var path = tenant?.Value?.Path?.ToUpper();
        if (TenantCollection == null || _existingTenants == null) return false;

        if (string.IsNullOrWhiteSpace(path))
            return false;

        return true;
    }

    private string GetTenantNameError(SnapshotEntity<BaseTenantDto> tenant)
    {
        var name = tenant?.Value?.Name?.ToUpper();
        if (TenantCollection == null || _existingTenants == null) return string.Empty;

        if (_existingTenants.Tenants.Any(t => t.Name?.ToUpper() == name))
            return "name already taken by existing tenant";
        if (TenantCollection.Any(t => t != tenant && t.Value?.Name?.ToUpper() == name))
            return "name already added in list";

        return string.Empty;
    }

    private string GetTenantPathError(SnapshotEntity<BaseTenantDto> tenant)
    {
        var path = tenant?.Value?.Path?.ToUpper();
        if (TenantCollection == null || _existingTenants == null) return string.Empty;

        if (_existingTenants.Tenants.Any(t => t.Path?.ToUpper() == path))
            return "path already taken by existing tenant";
        if (TenantCollection.Any(t => t != tenant && t.Value?.Path?.ToUpper() == path))
            return "path already added in list";

        return string.Empty;
    }

    #endregion

    private bool CanOpenDefaultsStep()
    {
        return (TenantCollection?.Count == 0 && SelectedExistingTenants ?.Count == 0) || _deploying;
    }

    private bool IsGroupValid()
    {
        return _groupNameIsValid && _groupPathIsValid && _groupUrlIsValid;
    }
}