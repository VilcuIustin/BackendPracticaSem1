using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class newImg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Images_ImgId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ImgId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImgId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Comments");

            migrationBuilder.AddColumn<long>(
                name: "ProfilePicId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ImageId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImgURL",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImgUrl = table.Column<string>(nullable: true),
                    PostId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImgURL", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImgURL_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfilePicId",
                table: "Users",
                column: "ProfilePicId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ImageId",
                table: "Comments",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImgURL_PostId",
                table: "ImgURL",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ImgURL_ImageId",
                table: "Comments",
                column: "ImageId",
                principalTable: "ImgURL",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ImgURL_ProfilePicId",
                table: "Users",
                column: "ProfilePicId",
                principalTable: "ImgURL",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ImgURL_ImageId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_ImgURL_ProfilePicId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ImgURL");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProfilePicId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ImageId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ProfilePicId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Comments");

            migrationBuilder.AddColumn<long>(
                name: "ImgId",
                table: "Posts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Comments",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ImgId",
                table: "Posts",
                column: "ImgId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Images_ImgId",
                table: "Posts",
                column: "ImgId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
