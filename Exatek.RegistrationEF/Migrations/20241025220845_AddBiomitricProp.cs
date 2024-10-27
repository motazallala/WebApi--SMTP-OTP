using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exatek.RegistrationEF.Migrations
{
    /// <inheritdoc />
    public partial class AddBiomitricProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "biometric",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "biometric",
                table: "Customers");
        }
    }
}
