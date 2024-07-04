using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WIL_v2_test.Data.Migrations
{
    /// <inheritdoc />
    public partial class aboutusupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "AboutUs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mission",
                table: "AboutUs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "AboutUs");

            migrationBuilder.DropColumn(
                name: "Mission",
                table: "AboutUs");
        }
    }
}
