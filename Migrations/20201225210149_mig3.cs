using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class mig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "UserId",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "UserId");

            migrationBuilder.AddColumn<long>(
                name: "UserId2",
                table: "UserId",
                type: "bigint",
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
    }
}
