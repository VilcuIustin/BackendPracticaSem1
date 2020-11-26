using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class migration7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Images_photoId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "photoId",
                table: "Posts",
                newName: "ImgId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_photoId",
                table: "Posts",
                newName: "IX_Posts_ImgId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NrLikes",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Images_ImgId",
                table: "Posts",
                column: "ImgId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UserId",
                table: "Users",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Images_ImgId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NrLikes",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "ImgId",
                table: "Posts",
                newName: "photoId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ImgId",
                table: "Posts",
                newName: "IX_Posts_photoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Images_photoId",
                table: "Posts",
                column: "photoId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
