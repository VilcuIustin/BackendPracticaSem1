using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class mig9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sended",
                table: "Friends");

            migrationBuilder.AddColumn<bool>(
                name: "sent",
                table: "Friends",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sent",
                table: "Friends");

            migrationBuilder.AddColumn<bool>(
                name: "sended",
                table: "Friends",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
