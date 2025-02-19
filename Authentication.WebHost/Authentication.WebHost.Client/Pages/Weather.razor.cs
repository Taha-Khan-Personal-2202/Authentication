using Authentication.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Authentication.WebHost.Client.Pages
{
    public partial class Weather
    {

        [Inject]
        AuthService AuthService { get; set; }

        [Inject]
        AuthenticationStateProvider AuthStateProvider { get; set; }


        public List<User> Users { get; set; }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Users = await AuthService.GeAllUesr();
                StateHasChanged();
            }
        }

        private async Task LogoutUser()
        {
            
            await AuthService.Logout();

            if (AuthStateProvider is CustomAuthStateProvider customAuthProvider)
            {
                // Notify authentication state change
                customAuthProvider.NotifyUserLogout();
            }

        }
    }
}
