using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtWebsite.Migrations
{
    /// <inheritdoc />
    public partial class LikeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Artworks_ArtworkId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Stories_StoryId",
                table: "Likes");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Artworks_ArtworkId",
                table: "Likes",
                column: "ArtworkId",
                principalTable: "Artworks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Stories_StoryId",
                table: "Likes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Artworks_ArtworkId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Stories_StoryId",
                table: "Likes");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Artworks_ArtworkId",
                table: "Likes",
                column: "ArtworkId",
                principalTable: "Artworks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Stories_StoryId",
                table: "Likes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id");
        }
    }
}
