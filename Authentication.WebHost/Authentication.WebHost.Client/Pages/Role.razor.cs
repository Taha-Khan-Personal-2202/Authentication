using Authentication.Shared.Model;
using Microsoft.AspNetCore.Components;

namespace Authentication.WebHost.Client.Pages
{
    public partial class Role
    {
        [Inject]
        RoleService RoleService { get; set; }

        private List<RoleViewModel> roles;
        private string newRoleName;
        private string editingRoleId;
        private string roleName;

        protected override async Task OnInitializedAsync()
        {
            await LoadRoles();
        }

        private async Task LoadRoles()
        {
            roles = await RoleService.GetAllRoles();
        }

        private async Task AddRole()
        {
            if (!string.IsNullOrWhiteSpace(newRoleName))
            {
                await RoleService.Add(newRoleName);
                newRoleName = "";
                await LoadRoles();
            }
        }

        private void EditRole(RoleViewModel role)
        {
            editingRoleId = role.Id;
            roleName = role.Name;
        }

        private async Task UpdateRole(string id)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                await RoleService.Update(id, roleName);
                editingRoleId = null;
                await LoadRoles();
            }
        }

        private void CancelEdit()
        {
            editingRoleId = null;
        }

        private async Task DeleteRole(string id)
        {
            await RoleService.DeleteRole(id);
            await LoadRoles();
        }


    }
}
