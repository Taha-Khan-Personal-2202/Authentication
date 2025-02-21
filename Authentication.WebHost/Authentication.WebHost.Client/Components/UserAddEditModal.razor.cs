using System.Reflection;
using Authentication.Shared.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Authentication.WebHost.Client.Components
{
    public partial class UserAddEditModal
    {

        [Inject]
        AuthService AuthService { get; set; }

        [Parameter]
        public string Email { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<bool> IsClosed { get; set; }


        public UserViewModel EditModel { get; set; } = new UserViewModel();

        bool isLoading = false;

        
        protected override async void OnParametersSet()
        {
            if (!string.IsNullOrEmpty(Email))
            {
                await GetByEmailId();
            }
            base.OnParametersSet();
        }

        async Task GetByEmailId()
        {
            var obj = await AuthService.GetByEmail(Email);
            EditModel = obj;
            StateHasChanged();
        }

        async Task Submit()
        {
            if (string.IsNullOrEmpty(Email))
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
            var isAdded = await AuthService.Register(EditModel);
            if (isAdded)
            {
                CloseModal();
            }
            isLoading = false;
            StateHasChanged();
        }

        async Task Update()
        {
            isLoading = true;
            var isAdded = await AuthService.Update(EditModel);
            if (isAdded)
            {
                CloseModal();
            }
            isLoading = false;
            StateHasChanged();
        }


        async void CloseModal()
        {
            await IsClosed.InvokeAsync();
        }

    }

}
