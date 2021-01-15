using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friend_User1id",
                table: "Friend");

            migrationBuilder.CreateIndex(
                name: "IX_Friend_User1id",
                table: "Friend",
                column: "User1id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friend_User1id",
                table: "Friend");

            migrationBuilder.CreateIndex(
                name: "IX_Friend_User1id",
                table: "Friend",
                column: "User1id",
                unique: true);
        }
    }
}
