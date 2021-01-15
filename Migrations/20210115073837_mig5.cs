using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class mig5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friend_Users_User1id",
                table: "Friend");

            migrationBuilder.RenameColumn(
                name: "User1id",
                table: "Friend",
                newName: "User1Id");

            migrationBuilder.RenameIndex(
                name: "IX_Friend_User1id",
                table: "Friend",
                newName: "IX_Friend_User1Id");

            migrationBuilder.AlterColumn<long>(
                name: "User1Id",
                table: "Friend",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Friend",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friend_UserId",
                table: "Friend",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_Users_User1Id",
                table: "Friend",
                column: "User1Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_Users_UserId",
                table: "Friend",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friend_Users_User1Id",
                table: "Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_Friend_Users_UserId",
                table: "Friend");

            migrationBuilder.DropIndex(
                name: "IX_Friend_UserId",
                table: "Friend");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Friend");

            migrationBuilder.RenameColumn(
                name: "User1Id",
                table: "Friend",
                newName: "User1id");

            migrationBuilder.RenameIndex(
                name: "IX_Friend_User1Id",
                table: "Friend",
                newName: "IX_Friend_User1id");

            migrationBuilder.AlterColumn<long>(
                name: "User1id",
                table: "Friend",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_Users_User1id",
                table: "Friend",
                column: "User1id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
