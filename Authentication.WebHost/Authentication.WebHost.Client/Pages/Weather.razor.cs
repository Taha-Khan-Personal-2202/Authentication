using System.Data;
using Authentication.Shared.Model;
using Authentication.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Authentication.WebHost.Client.Pages
{
    public partial class Weather
    {

        [Inject]
        IServiceProvider ServiceProvider { get; set; }

        [Inject]
        AuthService AuthService { get; set; }

        [Inject]
        AuthenticationStateProvider AuthStateProvider { get; set; }

        public UserViewModel Model { get; set; } = new();

        public List<UserViewModel> Users { get; set; } = new();

        public bool isLoading = false;

        bool isOpenModal = false;
        string selectedEmail = string.Empty;
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await CheckClaims();
                await GetAllUsers();
            }
        }

        async Task GetAllUsers()
        {
            Users = await AuthService.GeAllUser();
            CheckPolicy();
            StateHasChanged();
        }

        void OpenAddEditModal(string email)
        {
            selectedEmail = email;
            isOpenModal = !isOpenModal;
        }


        private async Task CheckClaims()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            Console.WriteLine("User Claims:");
            foreach (var claim in user.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }

            if (user.HasClaim(c => c.Type == "Permission" && c.Value == "Permissions.ManageUser"))
            {
                Console.WriteLine("✅ Permissions.ManageUser claim is present.");
            }
            else
            {
                Console.WriteLine("❌ Permissions.ManageUser claim is missing.");
            }
        }

        public bool CheckPolicy()
        {
            var policyProvider = ServiceProvider.GetRequiredService<IAuthorizationPolicyProvider>();
            var policy = policyProvider.GetPolicyAsync(Permission.ManageUser).GetAwaiter().GetResult();
            return policy != null;
        }


    }
}