﻿using Authentication.Shared.Model;
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

        public async Task LoginUser(UserViewModel obj)
        {
            isLoading = true;

            UserViewModel user = new()
            {
                Email = obj.Email,
                Password = obj.Password,
            };

            UserViewModel apiObject = await AuthService.Login(user);
            if (apiObject != null && !string.IsNullOrEmpty(apiObject.token))
            {
                // Cast to CustomAuthStateProvider before calling NotifyUserAuthentication
                if (AuthStateProvider is CustomAuthStateProvider customAuthProvider)
                {
                    customAuthProvider.NotifyUserAuthentication(apiObject.token, apiObject.Email, apiObject.FullName, apiObject.Role, apiObject.Permissions);
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