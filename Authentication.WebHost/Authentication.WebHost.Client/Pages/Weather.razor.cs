using System.Data;
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

        public User Model { get; set; } = new();

        public List<User> Users { get; set; }

        public bool isLoading = false;

        public string textLabel = "Add";

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

        async Task Submit()
        {
            if (textLabel == "Add")
            {
                await Add();
            }
            else
            {
                await Update();
            }
        }

        async Task Add()
        {
            isLoading = true;
            var isAdded = await AuthService.Register(Model);
            if (isAdded)
            {
                await GetAllUsers();
            }
            isLoading = false;
            StateHasChanged();
        }

        async Task Update()
        {
            isLoading = true;
            var isAdded = await AuthService.Update(Model);
            if (isAdded)
            {
                await GetAllUsers();
            }
            isLoading = false;
            StateHasChanged();
        }

        async Task GetByEmailId(string email)
        {
            var obj = await AuthService.GetByEmail(email);
            Model = obj;
            textLabel = "Update";
            StateHasChanged();
        }

    }
}
