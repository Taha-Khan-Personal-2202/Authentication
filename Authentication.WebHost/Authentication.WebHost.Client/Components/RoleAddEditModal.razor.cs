using Authentication.Shared.Model;
using Microsoft.AspNetCore.Components;

namespace Authentication.WebHost.Client.Components
{
    public partial class RoleAddEditModal
    {

        [Inject]
        RoleService RoleService { get; set; }

        [Parameter]
        public string Id { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<bool> IsClosed { get; set; }

        public RoleViewModel EditModel = new();
        public List<PermissionModel> permissionList = new();

        bool isLoading { get; set; }

        protected override async void OnParametersSet()
        {
            GetAllPermission();
            if (!string.IsNullOrEmpty(Id))
            {
                await GetByEmailId();
            }
            base.OnParametersSet();
        }

        async Task GetByEmailId()
        {
            isLoading = true;
            var obj = await RoleService.GetRoleById(Id);
            EditModel = obj;
            if (EditModel.PermissionIds != null && EditModel.PermissionIds.Any() && permissionList != null && permissionList.Any())
            {
                foreach (var permission in permissionList)
                {
                    permission.IsActive = EditModel.PermissionIds.Any(a => a == permission.Id);
                }
            }
            isLoading = false;
            StateHasChanged();
        }

        async Task Submit()
        {
            EditModel.PermissionIds = new();

            foreach (var item in permissionList)
            {
                if (item.IsActive)
                {
                    EditModel.PermissionIds.Add(item.Id);
                }
                else
                {
                    EditModel.PermissionIds.Remove(item.Id);
                }
            }

            if (string.IsNullOrEmpty(Id))
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
            var isAdded = await RoleService.Add(EditModel);
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
            var isAdded = await RoleService.Update(Id, EditModel);
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

        async void GetAllPermission()
        {
            permissionList = await RoleService.GetAllPermissions();
            StateHasChanged();
        }
    }

}
