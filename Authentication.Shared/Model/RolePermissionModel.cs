using Microsoft.AspNetCore.Identity;

namespace Authentication.Shared.Model
{
    public class RolePermissionModel
    {
        public int Id { get; set; }
        public string RoleId { get; set; } 
        public int PermissionId { get; set; } 

        public IdentityRole Role { get; set; }  
        public PermissionModel Permission { get; set; } 
    }
}
