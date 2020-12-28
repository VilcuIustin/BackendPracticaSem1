using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Backend.Migrations
{
    public partial class mig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    DTPost = table.Column<DateTime>(nullable: false),
                    NrLikes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ImageId = table.Column<long>(nullable: true),
                    CommentId = table.Column<long>(nullable: true),
                    PostId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_ImgURL_ImageId",
                        column: x => x.ImageId,
                        principalTable: "ImgURL",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    ProfilePicId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_ImgURL_ProfilePicId",
                        column: x => x.ProfilePicId,
                        principalTable: "ImgURL",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostId",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    postId = table.Column<long>(nullable: false),
                    userId = table.Column<long>(nullable: false),
                    MyPost = table.Column<long>(nullable: true),
                    idPost = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostId_Users_MyPost",
                        column: x => x.MyPost,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostId_Users_idPost",
                        column: x => x.idPost,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserId",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    followedBy = table.Column<long>(nullable: false),
                    following = table.Column<long>(nullable: false),
                    dateFollowing = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: true),
                    UserId1 = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserId", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserId_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserId_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentId",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ImageId",
                table: "Comments",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ImgURL_PostId",
                table: "ImgURL",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostId_MyPost",
                table: "PostId",
                column: "MyPost");

            migrationBuilder.CreateIndex(
                name: "IX_PostId_idPost",
                table: "PostId",
                column: "idPost");

            migrationBuilder.CreateIndex(
                name: "IX_UserId_UserId",
                table: "UserId",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserId_UserId1",
                table: "UserId",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfilePicId",
                table: "Users",
                column: "ProfilePicId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "PostId");

            migrationBuilder.DropTable(
                name: "UserId");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ImgURL");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
