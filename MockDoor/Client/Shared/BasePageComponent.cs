using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MockDoor.Client.State;

namespace MockDoor.Client.Shared;

public class BasePageComponent: ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    
    [Inject]
    protected PageHistoryState PageHistoryState { get; set; }

    public BasePageComponent(NavigationManager navManager, PageHistoryState pageState)
    {
        NavigationManager = navManager;
        PageHistoryState = pageState;
    }

    public BasePageComponent() { }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        PageHistoryState.AddPageToHistory(NavigationManager.Uri);
    }

    public void GoBackPage(bool checkReturnUrl = false)
    {
        string returnUrl = null;
        if (checkReturnUrl)
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("returnUrl", out var param))
            {
                returnUrl = param.First();
            }
        }
        
        if(string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = PageHistoryState.GetGoBackPage(true);
        }
        
        NavigationManager.NavigateTo(returnUrl);
    }
}