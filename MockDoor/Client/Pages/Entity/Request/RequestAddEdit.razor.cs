using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.JSInterop;
using MockDoor.Client.Services;
using MockDoor.Shared.Models.Headers;
using MockDoor.Shared.Models.QueryParameters;
using MockDoor.Shared.Models.ServiceRequest;
using MockDoor.Shared.Models.Utility;
using Radzen;
using Radzen.Blazor;

namespace MockDoor.Client.Pages.Entity.Request;

public partial class RequestAddEdit
{
    [Inject]
    private RequestService RequestService { get; set; }
        
    [Inject] 
    private DialogService DialogService { get; set; }
    
    [Inject] private IJSRuntime JsRuntime { get; set; }
        
    [Inject] private ContextMenuService ContextMenuService { get; set; }
    
    [Inject] private TooltipService TooltipService { get; set; }
    
    [Parameter]
    public int MicroserviceId { get; set; }
    
    [Parameter]
    public int TenantId { get; set; }

    [Parameter]
    public int Id { get; set; }

    QueryParameterDto _newQueryParam = new QueryParameterDto();
    ServiceRequestHeaderDto _newHeader = new ServiceRequestHeaderDto();
    
    ElementReference? _headerElementReference;

    bool IsEditMode => Id > 0;

    ServiceRequestDto _request;

    BadRequestResultDto _errorsOnSave;

    bool _deleting;
    
    bool _fromUrlDirty;

    bool _fromBodyDirty;

    bool _exactUrlMatchDirty;

    bool _expectAuthHeaderDirty;

    bool _restTypeDirty;
    
    bool _mockBehaviourDirty;

    bool _simulateTimeDirty;

    bool _createdDirty;

    bool _queryParamsDirty;
    
    bool _headersDirty;
    
    bool _enabledDirty;

    private RadzenDataGrid<QueryParameterDto> _queryParamGrid;
    private RadzenDataGrid<ServiceRequestHeaderDto> _headerGrid;

    readonly JsonPatchDocument<UpdateServiceRequestDto> _jsonPatchDocument = new();

    protected override async Task OnParametersSetAsync()
    {
        if (IsEditMode)
        {
            var response = await RequestService.GetRequestAsync(Id);
            
            if (response.IsSuccessStatusCode)
            {
                _request = response.Content;
            }
            else
            {
                _errorsOnSave = response.BadRequestResult;
            }
        }
        else
        {
            _request = new ServiceRequestDto()
            {
                MicroserviceId = MicroserviceId
            };
        }
    }
    
    private async Task AddQueryParam()
    {
        _queryParamsDirty = true;
        _request.QueryParameters ??= new List<QueryParameterDto>();
        
        _request.QueryParameters.Add(new QueryParameterDto() { Name = _newQueryParam.Name, Value = _newQueryParam.Value, OrderIndex = _request.QueryParameters.Count });
        _newQueryParam = new QueryParameterDto();
        await _queryParamGrid.Reload();
    }
    
    
    private async Task AddHeader()
    {
        _headersDirty = true;
        _request.RequestHeaders ??= new List<ServiceRequestHeaderDto>();
        
        _request.RequestHeaders.Add(new ServiceRequestHeaderDto() { Name = _newHeader.Name, Value = _newHeader.Value });
        _newHeader = new ServiceRequestHeaderDto();
        await _headerGrid.Reload();
    }

    async Task DeleteQueryParam(QueryParameterDto queryParameter)
    {
        _queryParamsDirty = true;
        
        var confirmed = await DialogService.Confirm("Are you sure?", "Confirm remove query parameter from list?",
            new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != null && confirmed.Value)
        {
            if (queryParameter != null)
            {
                _request.QueryParameters.Remove(queryParameter);
            }

            await _queryParamGrid.Reload();
        }
    }

    async Task DeleteHeader(ServiceRequestHeaderDto header)
    {
        _headersDirty = true;
        
        var confirmed = await DialogService.Confirm("Are you sure?", "Confirm remove header from list?",
            new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != null && confirmed.Value)
        {
            if (header != null)
            {
                _request.RequestHeaders.Remove(header);
            }

            await _headerGrid.Reload();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ResetFieldsModifiedState();
    }
    

    async Task DeleteRequestDialog()
    {
        var confirmed = await DialogService.Confirm("Are you sure?", "Confirm delete request?",
            new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != null && confirmed.Value)
        {
            _deleting = true;
            StateHasChanged();

            await Task.Run(async () =>
            {
                var response = await RequestService.DeleteRequestAsync(Id);

                if (response)
                    GoBackPage(true);
                
                _deleting = false;
                StateHasChanged();
            });
        }
    }

    private void ResetFieldsModifiedState()
    {
        _fromUrlDirty = false;
        _fromBodyDirty = false;
        _exactUrlMatchDirty = false;
        _expectAuthHeaderDirty = false;
        _restTypeDirty = false;
        _mockBehaviourDirty = false;
        _simulateTimeDirty = false;
        _createdDirty = false;
        _queryParamsDirty = false;
        _headersDirty = false;
        _enabledDirty = false;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(_request.FromBody))
            _request.FromBody = null;

        _errorsOnSave = null;
        
        if (IsEditMode)
        {
            if (_fromUrlDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.FromUrl, _request.FromUrl);

            if (_fromBodyDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.FromBody, string.IsNullOrWhiteSpace(_request.FromBody) ? null : _request.FromBody);

            if (_expectAuthHeaderDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.ExpectAuthHeader, _request.ExpectAuthHeader);

            if (_exactUrlMatchDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.ExactUrlMatch, _request.ExactUrlMatch);

            if (_restTypeDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.RestType, _request.RestType);

            if (_mockBehaviourDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.MockBehaviour, _request.MockBehaviour);

            if (_simulateTimeDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.SimulateTime, _request.SimulateTime);
            
            if(_createdDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.CreatedUtc, _request.CreatedUtc);
            
            if(_queryParamsDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.QueryParameters, _request.QueryParameters);

            if (_headersDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.RequestHeaders, _request.RequestHeaders);
            
            if(_enabledDirty)
                _jsonPatchDocument.Replace((updatedRequest) => updatedRequest.Enabled, _request.Enabled);
                

            var response = await RequestService.PatchRequestAsync(Id, _jsonPatchDocument);
            
            if (!response.IsSuccessStatusCode)
            {
                _errorsOnSave = response.BadRequestResult;
                return;
            }
            GoBackPage(true);
        }
        else
        {
            var response = await RequestService.CreateRequest(MicroserviceId, _request);
            
            if (!response.IsSuccessStatusCode)
            {
                _errorsOnSave = response.BadRequestResult;
                return;
            }
            NavigationManager.NavigateTo($"request/edit/{TenantId}/{response.Content.Id}");
        }
    }

    private bool RequestIsValid()
    {
        return _request != null && !string.IsNullOrWhiteSpace(_request.FromUrl);
    }
    
    async Task OnCopyHeaderValueAsync(string value)
    {
        await JsRuntime.InvokeVoidAsync("clipboardCopy.copyText", value);

        if (_headerElementReference != null)
        {
            TooltipService.Open(_headerElementReference.Value,
                "Copied header value to clipboard!",
                new TooltipOptions() { Position = TooltipPosition.Bottom, Duration = 1000 }); }

        ContextMenuService.Close();
    }
}