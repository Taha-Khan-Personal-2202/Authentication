﻿namespace Authentication.Shared.Model
{
    public class RoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string ConcurrencyStamp { get; set; } = string.Empty;
        public List<int> PermissionIds { get; set; }
    }
}
