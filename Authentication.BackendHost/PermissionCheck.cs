using Authentication.BackendHost.CustomServices;
using Authentication.BackendHost.DataBase;
using Authentication.Shared.Model;
using Microsoft.AspNetCore.Identity;

public class PermissionCheck
{
    private readonly CustomMethods _customMethods;

    public PermissionCheck(CustomMethods customMethods)
    {
        _customMethods = customMethods;
    }

    public async Task SeedPermissions(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        var permissions = _customMethods.GetPermission();

        foreach (var perm in permissions)
        {
            if (!context.Permissions.Any(p => p.Name == perm))
            {
                context.Permissions.Add(new PermissionModel { Name = perm });
            }
        }

        await context.SaveChangesAsync();
    }
}
