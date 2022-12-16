using Microsoft.AspNetCore.Components;

namespace MockDoor.Client.Shared.Component.Fieldset;

public partial class DetailedDatePicker
{
    [Parameter]
    public DateTime Value { get; set; }
    
    [Parameter]
    public EventCallback<DateTime> ValueChanged { get; set; }
    
    [Parameter]
    public string DateFormat { get; set; }
    
    [Parameter]
    public string CssClass { get; set; }
    
    [Parameter]
    public bool ShowTime { get; set; }
    
    [Parameter]
    public bool ShowSeconds { get; set; }
    
    [Parameter]
    public bool ShowMilliseconds { get; set; }
}