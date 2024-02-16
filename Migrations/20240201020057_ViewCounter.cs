using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtWebsite.Migrations
{
    /// <inheritdoc />
    public partial class ViewCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Artworks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "Artworks");
        }
    }
}
