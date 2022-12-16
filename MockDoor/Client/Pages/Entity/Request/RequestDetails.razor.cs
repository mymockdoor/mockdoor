using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MockDoor.Client.Services;
using MockDoor.Shared.Models.ServiceRequest;
using Radzen;

namespace MockDoor.Client.Pages.Entity.Request;

public partial class RequestDetails
{
    [Inject]
    private RequestService RequestService { get; set; }
    
    [Inject] private IJSRuntime JsRuntime { get; set; }
        
    [Inject] private ContextMenuService ContextMenuService { get; set; }
    
    [Inject] private TooltipService TooltipService { get; set; }
    
    [Parameter]
    public int TenantId { get; set; }

    [Parameter]
    public int Id { get; set; }
    
    ServiceRequestDto _request;
    
    ElementReference? _headerElementReference;
    
    protected override async Task OnParametersSetAsync()
    {
        var response = await RequestService.GetRequestAsync(Id);
        
        if (response.IsSuccessStatusCode)
        {
            _request = response.Content;
        }
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