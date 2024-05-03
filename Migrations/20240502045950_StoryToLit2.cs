using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtGalaxy.Migrations
{
    /// <inheritdoc />
    public partial class LiteratureToLit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Literature_LiteratureId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Literature_LiteratureId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "LiteratureId",
                table: "Likes",
                newName: "LiteratureId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_LiteratureId",
                table: "Likes",
                newName: "IX_Likes_LiteratureId");

            migrationBuilder.RenameColumn(
                name: "LiteratureId",
                table: "Comments",
                newName: "LiteratureId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_LiteratureId",
                table: "Comments",
                newName: "IX_Comments_LiteratureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Literature_LiteratureId",
                table: "Comments",
                column: "LiteratureId",
                principalTable: "Literature",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Literature_LiteratureId",
                table: "Likes",
                column: "LiteratureId",
                principalTable: "Literature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Literature_LiteratureId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Literature_LiteratureId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "LiteratureId",
                table: "Likes",
                newName: "LiteratureId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_LiteratureId",
                table: "Likes",
                newName: "IX_Likes_LiteratureId");

            migrationBuilder.RenameColumn(
                name: "LiteratureId",
                table: "Comments",
                newName: "LiteratureId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_LiteratureId",
                table: "Comments",
                newName: "IX_Comments_LiteratureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Literature_LiteratureId",
                table: "Comments",
                column: "LiteratureId",
                principalTable: "Literature",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Literature_LiteratureId",
                table: "Likes",
                column: "LiteratureId",
                principalTable: "Literature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
