using Authentication.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Authentication.WebHost.Client.Pages
{
    public partial class Login
    {
        [Inject]
        AuthService AuthService { get; set; }

        [Inject]
        NavigationManager Navigation { get; set; }

        [Inject]
        AuthenticationStateProvider AuthStateProvider { get; set; }

        public string email;
        public string password;
        public string errorMessage;

        public async Task LoginUser()
        {
            User user = new()
            {
                Email = email,
                Password = password,
                Name = email,
            };

            string token = await AuthService.Login(user);
            if (!string.IsNullOrEmpty(token))
            {
                // Cast to CustomAuthStateProvider before calling NotifyUserAuthentication
                if (AuthStateProvider is CustomAuthStateProvider customAuthProvider)
                {
                    customAuthProvider.NotifyUserAuthentication(token, user.Email);
                }

                Navigation.NavigateTo("/weather"); // Redirect to dashboard after login
            }
            else
            {
                errorMessage = "Invalid credentials!";
            }
        }


    }
}
