using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class mig7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fullname",
                table: "Friend");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fullname",
                table: "Friend",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
