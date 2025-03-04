using Authentication.BackendHost.DataBase;
using Authentication.Shared.Constants;
using Authentication.Shared.Model;
using Authentication.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace Authentication.BackendHost.CustomServices
{
    public class CustomMethods
    {
        public UserManager<User> UserManager { get; set; }
        public SignInManager<User> SignInManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }
        public ApplicationDbContext ApplicationDb { get; }

        public CustomMethods(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDb)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            ApplicationDb = applicationDb;
        }

        // CUSTOM METHODS
        public async Task<string> GetRoleByIdentityUser(User user)
        {
            var roles = await UserManager.GetRolesAsync(user);
            return roles != null && roles.Any() ? roles.FirstOrDefault() : string.Empty;
        }

        public async Task<List<string>> GetPermissionByIdentityUser(User user)
        {
            var claims = await UserManager.GetClaimsAsync(user);
            var permission = claims
                .Where(c => c.Type == Constant.PermissionClaimType)
                .Select(c => c.Value).ToList();

            return permission;
        }

        public async Task<bool> ChangePassword(User identityUser, UserViewModel userViewModel)
        {
            var changedPassword = await UserManager.ChangePasswordAsync(identityUser, userViewModel.Password, userViewModel.NewPassword);
            return changedPassword.Succeeded;
        }

        public async Task<bool> HandlingRoleAddOrUpdate(User identityUser, UserViewModel userViewModel)
        {
            var isRoleExist = await RoleManager.RoleExistsAsync(userViewModel.Role);
            if (!isRoleExist)
            {
                await RoleManager.CreateAsync(new IdentityRole(userViewModel.Role));
            }

            var roles = await GetRoleByIdentityUser(identityUser);

            if (roles != null && roles.Any())
            {
                await UserManager.RemoveFromRoleAsync(identityUser, roles); // Remove first role
            }
            await UserManager.AddToRoleAsync(identityUser, userViewModel.Role);
            return true;
        }

        public async Task<bool> HandlingPermissionAddOrUpdate(User identityUser, UserViewModel userViewModel)
        {
            var permissionIds = await GetAssignPermission(userViewModel.RoleId.ToString());

            if (permissionIds == null || !permissionIds.Any())
            {
                return false;
            }

            var permissions = await GetAssignPermissionNames(permissionIds);

            // Remove existing claims
            var existingPermissions = await GetPermissionByIdentityUser(identityUser);
            foreach (var item in existingPermissions)
            {
                await UserManager.RemoveClaimAsync(identityUser, new System.Security.Claims.Claim(Constant.PermissionClaimType, item));
            }

            // Add new claims
            foreach (var permission in permissions)
            {
                await UserManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim(Constant.PermissionClaimType, permission));
            }

            return true;
        }


        public async Task<(bool, string)> CheckPassword(User identityUser, UserViewModel userViewModel)
        {
            var result = await SignInManager.CheckPasswordSignInAsync(identityUser, userViewModel.Password, false);
            return (result.Succeeded, Constant.MessageForWrongPassword);
        }

        public async Task<User>? FindUserByEmail(string email)
        {
            // GETTING THE EXISTING USER BY EMAIL
            return await UserManager.FindByEmailAsync(email);
        }

        public List<string> GetPermission()
        {
            var permission = new List<string>();
            permission.Add(Constant.ManageUser);
            permission.Add(Constant.ViewReports);

            return permission;
        }

        public async Task<List<int>> GetAssignPermission(string roleId)
        {
            var assignedPermissions = await ApplicationDb.RolePermissions
                                        .Where(rp => rp.RoleId == roleId)
                                        .Select(rp => rp.PermissionId)
                                        .ToListAsync();

            return assignedPermissions;
        }

        public async Task<List<string>> GetAssignPermissionNames(List<int> permissionIds)
        {
            var assignedPermissions = await ApplicationDb.Permissions
                                        .Where(p => permissionIds.Contains(p.Id))
                                        .Select(pn => pn.Name)
                                        .ToListAsync();

            return assignedPermissions;
        }

        public async Task<Dictionary<string, List<string>>> GetRolePermissionsAsync()
        {
            return await ApplicationDb.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .GroupBy(rp => rp.Role.Name)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Select(rp => rp.Permission.Name).ToList()
                );
        }

        public async Task<string> GetRoleNameById(string id)
        {
            return ApplicationDb.Roles.FirstOrDefault(f => f.Id == id)?.Name!;
        }

        public async Task<string> GetRoleIdByName(string name)
        {
            return ApplicationDb.Roles.FirstOrDefault(f => f.Name == name)?.Id!;
        }

        public async Task<List<string>> GetPermissionByRoleName(string name)
        {
            var id = ApplicationDb.Roles.FirstOrDefault(r => r.Name == name)?.Id;
            var permissionIds = await GetAssignPermission(id);
            var permissions = await GetAssignPermissionNames(permissionIds);
            return permissions;

        }
    }
}
