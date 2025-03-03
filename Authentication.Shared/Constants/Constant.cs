namespace Authentication.Shared.Constants
{
    public static class Constant
    {
        public static string PermissionClaimType = "Permission";
        public static string MessageForUserNotFound = "User not found.";
        public static string MessageForPasswordUpdate = "Could not change the password.";
        public static string MessageForWrongPassword = "Invalid Password.";
        public static string MessageForRole = "Role could not be updated.";
        public static string MessageForPermissionError = "Could not add or update permission try to check the role and its permission added.";


        public static string ManageUser = "Permissions.ManageUser";
        public static string ViewReports = "Permissions.ViewReports";
    }
}
