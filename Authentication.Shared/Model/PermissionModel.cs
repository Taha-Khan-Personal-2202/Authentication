using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Shared.Model
{
    public class PermissionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public bool IsActive { get; set; }
    }
}
