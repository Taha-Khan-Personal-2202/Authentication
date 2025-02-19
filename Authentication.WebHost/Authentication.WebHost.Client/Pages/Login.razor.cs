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

        private bool isPasswordVisible = false;

        private string passwordInputType => isPasswordVisible ? "text" : "password";

        private bool isLoading = false;

        


        private void TogglePasswordVisibility()
        {
            isPasswordVisible = !isPasswordVisible;
        }


        public async Task LoginUser()
        {
            isLoading = true;

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
            isLoading = false;
        }


    }
}
