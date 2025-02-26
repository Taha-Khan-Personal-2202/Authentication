using Authentication.BackendHost.DataBase;
using Authentication.Shared.Constants;
using Authentication.Shared.Model;
using Authentication.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace Authentication.BackendHost.CustomServices
{
    public class CustomMethods
    {
        public UserManager<User> UserManager { get; set; }
        public SignInManager<User> SignInManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public CustomMethods(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
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
            await UserManager.AddToRoleAsync(identityUser, userViewModel.Role);
            return true;
        }

        public async Task<bool> HandlingPermissionAddOrUpdate(User identityUser, UserViewModel userViewModel)
        {
            if (RolePermissions.RolePermissionMaping.ContainsKey(userViewModel.Role))
            {
                foreach (var permission in RolePermissions.RolePermissionMaping[userViewModel.Role])
                {
                    await UserManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim(Constant.PermissionClaimType, permission));
                }
            }
            else
            {
                return false;
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

    }
}
