namespace Authentication.Shared.Model
{
    public class RolePermissions
    {
        public static Dictionary<string, List<string>> RolePermissionMaping = new()
        {
            {"Admin", new List<string>{ Permission.ManageUser} },
            {"Agent", new List<string>{ Permission.ViewReports} },
            {"SuperAdmin", new List<string>{ Permission.ViewReports, Permission.ManageUser} }
        };
    }
}
