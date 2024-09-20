using Microsoft.AspNetCore.Components;

namespace PBS.DSS.WebServices.Client.Services
{
    public class NavigationManagerService(NavigationManager navigationManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;

        public void NavigateTo(string uri) => _navigationManager.NavigateTo(uri);
        public void Refresh() => _navigationManager.Refresh();
    }
}
