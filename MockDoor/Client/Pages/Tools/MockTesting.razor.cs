using System.Diagnostics;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MockDoor.Client.Constants;
using MockDoor.Client.Helpers;
using MockDoor.Client.Services;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.ServiceGroup;
using MockDoor.Shared.Models.ServiceRequest;
using MockDoor.Shared.Models.Tenant;
using Radzen;
using Radzen.Blazor;

namespace MockDoor.Client.Pages.Tools
{
    public partial class MockTesting
    {
        [Inject]
        private DialogService DialogService { get; set; }

        [Inject] 
        public ILocalStorageService LocalStorage { get; set; }
        
        [Inject] 
        public TooltipService TooltipService{ get; set; }

        [Inject] 
        public ContextMenuService ContextMenuService{ get; set; }

        [Inject]
        public MockTestingService MockTestingService { get; set; }

        [Inject] 
        public IJSRuntime JsRuntime { get; set; }

        private string _pathOrder = "tgm";

        private string _testUrl, _testBody = string.Empty;

        private RestType _currentRestType = RestType.GET;

        private string _testResult, _domain;

        private string _newHeaderName, _newHeaderValue;

        private int? _responseCode;

        private List<HeaderItem> _headers = new ();
        private List<HeaderItem> _responseHeaders = new ();

        private RadzenDataGrid<HeaderItem> _headersGrid;

        private BaseTenantDto SelectedTenant { get; set; }

        private BasicServiceGroupDto SelectedServiceGroup { get; set; }

        private MicroserviceResultDto SelectedMicroservice { get; set; }
        
        private ElementReference? _mocktestingendpointrow, _headerElementReference;

        private List<string> _parameterOrderList = new List<string>
        {
            "tgm",
            "mtg",
            "gmt",
            "mgt",
            "tmg",
            "gtm"
        };

        private long? _elapsedTime;
        private int _timeout;
        private bool _isBusy;

        private string ReturnUrl
        {
            get
            {
                string returnParams = string.Empty;

                if (SelectedTenant != null)
                {
                    returnParams += $"?tenantId={SelectedTenant.Id}";
                }

                if (SelectedServiceGroup != null)
                {
                    returnParams += $"&serviceGroupId={SelectedServiceGroup.Id}";
                }

                if (SelectedMicroservice != null)
                {
                    returnParams += $"&microserviceId={SelectedMicroservice.Id}";
                }

                return System.Web.HttpUtility.UrlEncode($"mock-testing{returnParams}");
            }
        }
        
        protected override async Task OnInitializedAsync()
        {
            var prePopulateRequest =
                await LocalStorage.GetItemAsync<ServiceRequestDto>(UiConstants.MockTestingPopulateKey);

            if (prePopulateRequest != null)
            {
                var targetUrl = new StringBuilder(prePopulateRequest.FromUrl);
                
                if (prePopulateRequest.QueryParameters?.Any() ?? false)
                {
                    targetUrl.Append("?");
                    targetUrl.Append(string.Join('&', prePopulateRequest.QueryParameters.Select(qp => $"{qp.Name}={qp.Value}")));
                }

                _testUrl = targetUrl.ToString();
                _currentRestType = prePopulateRequest.RestType;
                _testBody = prePopulateRequest.FromBody;
                var prePopulateHeaders = prePopulateRequest.RequestHeaders?.Select(h => new HeaderItem() { Name = h.Name, Value = string.Join(';', h.Value) }).ToList();

                if (prePopulateHeaders != null)
                {
                    _headers = prePopulateHeaders;
                }

                await LocalStorage.RemoveItemAsync(UiConstants.MockTestingPopulateKey);
            }
            
            _domain = await LocalStorage.GetItemAsStringAsync("domain");
        }
        
        private async Task EnterOnTestAsync(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
                if (!DisableTest())
                {
                    await TestAsync(null);
                }
                    
        }
        
        async Task OnCopyUrlAsync()
        {
            await JsRuntime.InvokeVoidAsync("clipboardCopy.copyText", $"{GetDomain()}api/mock/{_pathOrder}{GetUrlPathSegments()}/{_testUrl?.TrimStart('/') ?? ""}");

            if (_mocktestingendpointrow != null)
            {
                TooltipService.Open(_mocktestingendpointrow.Value,
                    "Copied url to clipboard!",
                    new TooltipOptions() { Position = TooltipPosition.Bottom, Duration = 1000 }); }

            ContextMenuService.Close();
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

        void ShowTooltip(ElementReference? elementReference, TooltipOptions options = null)
        {
            if (elementReference == null || elementReference.Value.Context == null) 
            {
                Console.WriteLine("Error null element ref");
                return;
            }
            TooltipService.Open(elementReference.Value,
                "Example parameter order. t = tenant, g = group and m = microservice.", options);
        }

        private string GetUrlPathSegments()
        {
            string firstPathSegment = ConvertCharToPathSegment(_pathOrder.ToCharArray()[0]);
            string secondPathSegment = ConvertCharToPathSegment(_pathOrder.ToCharArray()[1]);
            string thirdPathSegment = ConvertCharToPathSegment(_pathOrder.ToCharArray()[2]);

            return $"/{firstPathSegment}/{secondPathSegment}/{thirdPathSegment}";
        }

        private async Task AddHeaderAsync()
        {
            _headers.Add(new HeaderItem(_newHeaderName, _newHeaderValue));
            _newHeaderName = string.Empty;
            _newHeaderValue = string.Empty;
            await _headersGrid.Reload();
        }
        
        async Task DeleteHeaderAsync(HeaderItem header)
        {
            var confirmed = await DialogService.Confirm("Are you sure?", "Confirm remove from list?",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed != null && confirmed.Value)
            {
                if (header != null)
                {
                    _headers.Remove(header);
                }

                await _headersGrid.Reload();
            }
        }

        private string ConvertCharToPathSegment(char c)
        {
            switch (c)
            {
                case 't': return SelectedTenant?.Path;
                case 'g': return SelectedServiceGroup?.Path;
                default: return SelectedMicroservice?.Path;
            }
        }

        private string GetDomain()
        {
            if(_domain == null)
            {
                return "loading...";
            }
            if (_domain.EndsWith('/'))
                return _domain;

            return $"{_domain}/";
        }

        private bool DisableTest()
        {
            return SelectedMicroservice == null || string.IsNullOrWhiteSpace(_testUrl) || (SelectedMicroservice.ProxyMode && string.IsNullOrWhiteSpace(SelectedMicroservice.TargetUrl));
        }

        private async Task TestAsync(MouseEventArgs args)
        {
            _isBusy = true;
            var stopwatch = Stopwatch.StartNew();

            var result = await MockTestingService.TestUrlAsync($"api/mock/{_pathOrder}{GetUrlPathSegments()}/{_testUrl.TrimStart('/')}", _currentRestType, _testBody, _headers, timeout: _timeout);

            stopwatch.Stop();
            
            _elapsedTime = stopwatch.ElapsedMilliseconds;

            if (result.IsSuccessStatusCode)
            {
                _testResult = await result.OriginalResponse.GetContentAsync();

                _responseHeaders = result.OriginalResponse.Headers
                    .Select(h => new HeaderItem() { Name = h.Key, Value = string.Join("; ", h.Value) }).ToList();
                
                _responseCode = (int)result.OriginalResponse.StatusCode;
            }
            else
            {
                _testResult = await result.OriginalResponse.GetContentAsync();

                if (string.IsNullOrWhiteSpace(_testResult))
                {
                    _testResult = "[No response body returned]";
                }
                
                _responseCode = (int)result.OriginalResponse.StatusCode;
            }

            _isBusy = false;
        }

        private Action<BasicServiceGroupDto> OnChangeSelectedGroup()
        {
            return arg => SelectedServiceGroup = arg;
        }
    }
}
