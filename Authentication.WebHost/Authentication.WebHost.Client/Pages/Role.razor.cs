using Authentication.Shared.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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

        public bool showEditModel = false;
        
        protected override async Task OnInitializedAsync()
        {
            await LoadRoles();
        }

        private async Task LoadRoles()
        {
            roles = await RoleService.GetAllRoles();
            StateHasChanged();
        }

        private void EditRole(string id)
        {
            editingRoleId = id;
            showEditModel = true;
        }

        private async Task DeleteRole(string id)
        {
            await RoleService.DeleteRole(id);
            await LoadRoles();
        }

    }
}
