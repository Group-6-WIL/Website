using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WIL_v2_test.Data.Migrations
{
    /// <inheritdoc />
    public partial class locationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Locations");
        }
    }
}
