using System.Reflection;
using Authentication.Shared.Model;
using Authentication.WebHost.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Authentication.WebHost.Client.Components
{
    public partial class UserAddEditModal
    {

        [Inject]
        AuthService AuthService { get; set; }

        [Inject]
        RoleService RoleService { get; set; }

        [Parameter]
        public string Email { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<bool> IsClosed { get; set; }


        public UserViewModel EditModel { get; set; } = new UserViewModel();
        public List<RoleViewModel> RoleList { get; set; } = new List<RoleViewModel>();

        bool isLoading = false;

        protected override async void OnInitialized()
        {
            await GetAllRoles();
            base.OnInitialized();
        }

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

        async Task GetAllRoles()
        {
            var obj = await RoleService.GetAllRoles();
            RoleList = obj;
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
            var isAdded = await AuthService.Add(EditModel);
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

        void GetRoleName(ChangeEventArgs e)
        {
            var value = e.Value.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                var name = RoleList.FirstOrDefault(f => f.Id == value).Name;
                EditModel.Role = name;
            }
        }

    }

}
