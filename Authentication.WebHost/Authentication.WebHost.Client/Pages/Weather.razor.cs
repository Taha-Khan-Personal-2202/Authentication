using System.Data;
using Authentication.Shared.Model;
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

        public UserViewModel Model { get; set; } = new();

        public List<UserViewModel> Users { get; set; }

        public bool isLoading = false;

        bool isOpenModal = false;
        string selectedEmail = string.Empty;
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await GetAllUsers();
            }
        }

        async Task GetAllUsers()
        {
            Users = await AuthService.GeAllUesr();
            StateHasChanged();

        }

        void OpenAddEditModal(string email)
        {
            selectedEmail = email;
            isOpenModal = !isOpenModal;
        }
        
    }
}
