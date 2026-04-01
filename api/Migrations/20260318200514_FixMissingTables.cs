using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestuarantAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "cart",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "cart");
        }
    }
}
