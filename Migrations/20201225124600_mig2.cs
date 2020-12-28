using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId2",
                table: "UserId",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserId_UserId2",
                table: "UserId",
                column: "UserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_UserId_Users_UserId2",
                table: "UserId",
                column: "UserId2",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserId_Users_UserId2",
                table: "UserId");

            migrationBuilder.DropIndex(
                name: "IX_UserId_UserId2",
                table: "UserId");

            migrationBuilder.DropColumn(
                name: "UserId2",
                table: "UserId");
        }
    }
}
