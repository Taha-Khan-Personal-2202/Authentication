using Microsoft.AspNetCore.Authorization;

namespace Authentication.BackendHost.CustomServices
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission)
        {
                Permission = permission;
        }
    }
}
