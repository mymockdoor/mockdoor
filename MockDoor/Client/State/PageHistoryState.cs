namespace MockDoor.Client.State;

public class PageHistoryState
{
    private Stack<string> _previousPages;
    private readonly string _fallbackPage;
    
    public PageHistoryState()
    {
        _previousPages = new Stack<string>();
        _fallbackPage = "home";
    }

    public void AddPageToHistory(string pageName)
    {
        _previousPages.Push(pageName);
    }

    public string GetGoBackPage(bool removeFromHistory = false)
    {
        // Check if is the first loaded page "/"
        if (_previousPages.TryPeek(out string url) && !string.IsNullOrWhiteSpace(url))
        {
            // If moved to the next page
            if (_previousPages.Count > 1)
            {
                if (removeFromHistory)
                {
                    //pop current page
                    _previousPages.Pop();
                    
                    // get and pop previous page
                    url = _previousPages.Pop();
                }
                else
                {
                    url = _previousPages.Skip(1).Last();
                }

                return url;
            }
        }

        // If stack is empty redirect to the fallback
        return _fallbackPage;
    }

    /// <summary>
    /// Removes latest page from history
    /// </summary>
    /// <returns>true if successful, false if no history to remove</returns>
    public bool RemoveLatestPage()
    {
        // If moved to the next page
        if (_previousPages.Count > 1)
        {
            _previousPages.Pop();
            return true;
        }

        return false;
    }
    
    public bool CanGoBack() => _previousPages.Count > 1;
}