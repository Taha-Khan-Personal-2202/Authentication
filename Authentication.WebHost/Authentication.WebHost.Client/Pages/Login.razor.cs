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

        public string errorMessage;
        
        private bool isLoading = false;

        public async Task LoginUser(User obj)
        {
            isLoading = true;

            User user = new()
            {
                Email = obj.Email,
                Password = obj.Password,
                UserName = obj.Email,
            };

            string token = await AuthService.Login(user);
            if (!string.IsNullOrEmpty(token))
            {
                // Cast to CustomAuthStateProvider before calling NotifyUserAuthentication
                if (AuthStateProvider is CustomAuthStateProvider customAuthProvider)
                {
                    customAuthProvider.NotifyUserAuthentication(token, user.Email);
                }

                Navigation.NavigateTo("/home"); // Redirect to dashboard after login
            }
            else
            {
                errorMessage = "Invalid credentials!";
            }
            isLoading = false;
        }


    }
}
