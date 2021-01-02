using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class notification2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "newNotifications",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "newNotifications",
                table: "Users");
        }
    }
}
