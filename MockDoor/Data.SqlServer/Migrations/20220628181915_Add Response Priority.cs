using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockDoor.Data.Migrations.SqlServer
{
    public partial class AddResponsePriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RequestResponse",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "RequestResponse",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "RequestResponse",
                type: "int",
                nullable: false,
                defaultValue: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "RequestResponse");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "RequestResponse");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "RequestResponse");
        }
    }
}
