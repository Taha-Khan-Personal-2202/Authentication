using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.BackendHost.Migrations
{
    /// <inheritdoc />
    public partial class AddNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ApplicationUsers",
                newName: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "ApplicationUsers",
                newName: "Name");
        }
    }
}
