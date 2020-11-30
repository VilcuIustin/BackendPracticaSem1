using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class mig4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId1",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId1",
                table: "Users",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UserId1",
                table: "Users",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UserId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Users");
        }
    }
}
