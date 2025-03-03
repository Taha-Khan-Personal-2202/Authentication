using Authentication.BackendHost.CustomServices;
using Authentication.Shared.Model;
using Authentication.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.BackendHost.DataBase
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<PermissionModel> Permissions { get; set; }
        public DbSet<RolePermissionModel> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RolePermissionModel>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RolePermissionModel>() // Entity ke pass hai
                .HasOne(rp => rp.Permission) // Aik permission model
                .WithMany() // Jo ke related hai multiple record se (Aur ye many to many relation ship hai)
                .HasForeignKey(rp => rp.PermissionId) // Permission Id ke base par
                .OnDelete(DeleteBehavior.Cascade); // Agar permission id delete howi to us se related object bhi delete ho jayenga


            base.OnModelCreating(builder);
        }
    }
}
