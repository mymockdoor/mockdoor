using System.Net;
using Microsoft.AspNetCore.Components;
using MockDoor.Client.Helpers;
using MockDoor.Client.Models;
using MockDoor.Client.Pages.Entity.Response;
using MockDoor.Client.Services;
using MockDoor.Shared;
using MockDoor.Shared.Models.Response;
using Radzen;
using Radzen.Blazor;

namespace MockDoor.Client.Shared.Component.DataGrid
{
    public partial class EditableResponseDataGrid
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        
        [Inject]
        DialogService DialogService { get; set; }

        [Inject]
        ResponseService ResponseService { get; set; }
        
        [Inject]
        public SimulateTimeService SimulateTimeService { get; set; }
        
        [Parameter]
        public int RequestId { get; set; }
        
        [Parameter]
        public Density Density { get; set; }

        [Parameter]
        public List<MockResponseDto> DataSource { get; set; }
        
        [Parameter]
        public DateTime? CurrentSimulationTime { get; set; }
        
        [Parameter]
        public EventCallback<int> RequestUpdated { get; set; }

        RadzenDataGrid<SnapshotEntity<MockResponseDto>> _responseGrid;

        List<SnapshotEntity<MockResponseDto>> _mockResponses;

        List<SnapshotEntity<MockResponseDto>> _selectedMockResponses = new ();

        string _errorMessage = null;
        int _minRows = 3, _maxRows = 25;
        private bool _busy, _updating;
        private int? filteredCurrentSimulationId;
        private bool? filteredEnabled;

        private bool IsEditing()
        {
            if (_responseGrid == null)
                return false;

            return _mockResponses?.Any(rr => _responseGrid.IsRowInEditMode(rr)) ?? false;
        }

        private static IEnumerable<EnumItem<HttpStatusCode>> GetStatusCodeEnums(bool includeAll = false)
        {
            return HelperMethodsExtensions.GetEnumList<HttpStatusCode>(includeAll);
        }

        private async Task<bool> SaveChangesAsync()
        {
            _updating = true;
            var response = await ResponseService.UpdateResponsesOnRequestAsync(RequestId, _mockResponses.Select(r => r.Value));

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = response.Message;
                
                _updating = false;
                return false;
            }
            
            if (response.Content == null)
            {
                _errorMessage = "Error request not found after updating mock responses";
                _updating = false;
                return false;
            }
            
            _errorMessage = null;
            _mockResponses.Clear();
            _mockResponses.AddRange(response.Content.MockResponses.Select(rr => new SnapshotEntity<MockResponseDto>(rr)));
            _selectedMockResponses.Clear();
            await _responseGrid.Reload();
            _updating = false;
            return true;
        }

        private async Task<bool> SaveChangeAsync(SnapshotEntity<MockResponseDto> responseDto)
        {
            _updating = true;
            var response = await ResponseService.UpdateResponseOnRequestAsync(RequestId, responseDto.Value);

            if (response.IsSuccessStatusCode)
            {
                _errorMessage = null;

                await _responseGrid.UpdateRow(responseDto);
                
                await RequestUpdated.InvokeAsync(RequestId);
                
                _updating = false;
                return true;
            }

            _updating = false;
            _errorMessage = response.Message;
            return false;
        }

        private async Task DeleteResponseAsync(SnapshotEntity<MockResponseDto> responseDto)
        {
            _updating = true;
            var response = await ResponseService.DeleteResponseAsync(responseDto.Value.Id);

            if (response)
            {
                _errorMessage = null;
                _mockResponses.Remove(responseDto);
                _selectedMockResponses.Remove(responseDto);

                await RequestUpdated.InvokeAsync(RequestId);
                await _responseGrid.Reload();
                StateHasChanged();
            }
            _updating = false;
        }

        private int CalculateSize(string value)
        {
            int bodyRows;
            if (string.IsNullOrWhiteSpace(value))
            {
                bodyRows = _minRows;
            }
            else
            {
                bodyRows = Math.Max(value.Split('\n').Length, value.Split('\r').Length) + 1;
                bodyRows = Math.Max(bodyRows, _minRows);
                bodyRows = Math.Min(bodyRows, _maxRows);
            }

            return bodyRows;
        }

        protected override void OnParametersSet()
        {
            _mockResponses = DataSource.Select(rr => new SnapshotEntity<MockResponseDto>(rr)).ToList();
        }

        async Task DuplicateSelectedAsync()
        {
            if (_selectedMockResponses?.Count > 0)
            {
                var duplicateResponses = new List<SnapshotEntity<MockResponseDto>>();

                // Copy over list initial data
                foreach (var selectedResponse in _selectedMockResponses)
                {
                    var dup = new MockResponseDto();
                    selectedResponse.Value.CopyTo(dup);
                    duplicateResponses.Add(new SnapshotEntity<MockResponseDto>(dup));
                }

                // set as new and append copy to description and body
                foreach (var duplicateResponse in duplicateResponses)
                {
                    duplicateResponse.Value.Id = 0;
                    duplicateResponse.Value.Description = duplicateResponse.Value.Description == null ? "copy" : duplicateResponse.Value.Description + " - copy";
                    duplicateResponse.Value.Body = duplicateResponse.Value.Body == null ? "copy" : duplicateResponse.Value.Body + " - copy";
                    duplicateResponse.CommitChanges();

                    await _responseGrid.InsertRow(duplicateResponse);
                }
            }
        }

        async Task ConfirmDeleteAllAsync()
        {
            var confirmed = await DialogService.Confirm("Are you sure?", "Confirm Delete?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed != null && confirmed.Value)
            {
                await DeleteSelectedAsync(_selectedMockResponses);
                await SaveChangesAsync();
            }
        }

        async Task ConfirmDeleteRowAsync(SnapshotEntity<MockResponseDto> mockResponse)
        {
            var confirmed = await DialogService.Confirm("Are you sure?", "Confirm Delete?", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed != null && confirmed.Value)
            {
                await DeleteSelectedAsync(mockResponse);
                
                if (mockResponse.Value.Id > 0)
                {
                    await DeleteResponseAsync(mockResponse);
                }
            }
        }
        
        async Task DeleteSelectedAsync(SnapshotEntity<MockResponseDto> itemToDelete)
        {
            if (_responseGrid.IsRowInEditMode(itemToDelete))
            {
                await CancelEditAsync(itemToDelete);
            }

            _mockResponses.Remove(itemToDelete);
            
            _selectedMockResponses.Clear();

            _errorMessage = null;           
        }

        async Task DeleteSelectedAsync(List<SnapshotEntity<MockResponseDto>> itemsToDelete)
        {
            foreach (var item in itemsToDelete)
            {
                if (_responseGrid.IsRowInEditMode(item))
                {
                    await CancelEditAsync(item);
                }

                _mockResponses.Remove(item);
            }

            _selectedMockResponses.Clear();

            _errorMessage = null;           
        }

        async Task EditSelectedRowsAsync()
        {
            foreach (var item in _mockResponses)
            {
                if (_responseGrid.IsRowInEditMode(item) && !_selectedMockResponses.Contains(item))
                {
                    await CancelEditAsync(item);
                }
                else if (_selectedMockResponses.Contains(item))
                {
                    await EditRowAsync(item);
                    await _responseGrid.SelectRow(item);
                }
            }
        }

        async Task CommitAllAsync()
        {
            foreach (var mockResponse in _mockResponses.Where(rr => _responseGrid.IsRowInEditMode(rr)))
            {
                await SaveRowAsync(mockResponse);
            }
            await SaveChangesAsync();
        }

        async Task CancelAllAsync()
        {
            foreach (var mockResponse in _mockResponses.Where(rr => _responseGrid.IsRowInEditMode(rr)))
            {
                await CancelEditAsync(mockResponse);
            }
        }

        void OnRowSelected(SnapshotEntity<MockResponseDto> response)
        {
            _selectedMockResponses.Add(response);
        }

        void OnRowDeselected(SnapshotEntity<MockResponseDto> response)
        {
            _selectedMockResponses.Remove(response);
        }

        async Task SaveAsync(SnapshotEntity<MockResponseDto> response)
        {
            await SaveRowAsync(response);
        }

        async Task SaveRowAsync(SnapshotEntity<MockResponseDto> response)
        {
            response.CommitChanges();
            await OnUpdateRow(response);
        }
        
        async Task OnUpdateRow(SnapshotEntity<MockResponseDto> mockResponse)
        {
            await SaveChangeAsync(mockResponse);
            _errorMessage = null;
        }

        async Task OnClickEditSplitButton(RadzenSplitButtonItem item, SnapshotEntity<MockResponseDto> response)
        {
            if (item is { Value: "EditPage" })
            {
                NavigationManager.NavigateTo($"response/edit/{response.Value.ServiceRequestId}/{response.Value.Id}?returnUrl={NavigationManager.Uri}");
            }
            else
            {
                await EditRowAsync(response);
            }
        }
        
        
        async Task EditRowAsync(SnapshotEntity<MockResponseDto> response)
        {
            await _responseGrid.EditRow(response);
            CalculateSize(response.Value.Body);
        }

        async Task CancelEditAsync(SnapshotEntity<MockResponseDto> response)
        {
            response.RestoreValue();
            _responseGrid.CancelEditRow(response);

            if (response.Value.Id == 0)
            { 
                _mockResponses.Remove(response);
                _selectedMockResponses.Remove(response);

                await _responseGrid.Reload();
            }

        }
        
        private bool IsResponseValid(SnapshotEntity<MockResponseDto> response)
        {
            if (response?.Value == null)
                return false;

            return response.Value.IsValid();
        }
        
        private async Task SetResponseAsSimulationTimeAsync(MockResponseDto mockResponse)
        {
            _busy = true;
            var updateResponse =
                await SimulateTimeService.SetSimulateTime(RequestId, mockResponse.Id == CurrentSimulatedResponseId() ?
                                                                                null : mockResponse.CreatedUtc,
                    TimeTravelScope.Request);

            if (updateResponse.IsSuccessStatusCode)
            {
                CurrentSimulationTime = mockResponse.CreatedUtc;
                await RequestUpdated.InvokeAsync(RequestId);
                await _responseGrid.Reload();
                StateHasChanged();
            }

            _busy = false;
        }

        private string GetSetSimulateButtonText(SnapshotEntity<MockResponseDto> response)
        {
            return CurrentSimulatedResponseId() == response.Value.Id
                ? "Clear Current response"
                : "Set as current response";
        }

        private int? CurrentSimulatedResponseId()
        {
            if (CurrentSimulationTime == null)
                return null;
            
            var resolvedSimulateTime = CurrentSimulationTime?.AddSeconds(1);

            var enabledResponses = _mockResponses.Where(r => r.Value.Enabled)
                .Where(er => resolvedSimulateTime == null || er.Value.CreatedUtc < resolvedSimulateTime)
                .ToList();
            
            var filteredAndOrderedResponses = enabledResponses
                                                            .OrderBy(er => er.Value.Priority)
                                                            .ThenByDescending(er => er.Value.CreatedUtc)
                                                            .ToList();

            if (filteredAndOrderedResponses.Count > 0)
            {
                return filteredAndOrderedResponses.First().Value.Id;
            }

            return null;
        }
    }
}
