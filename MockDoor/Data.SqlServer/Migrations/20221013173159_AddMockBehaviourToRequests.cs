using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockDoor.Data.Migrations.SqlServer
{
    public partial class AddMockBehaviourToRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MockBehaviour",
                table: "ServiceRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MockBehaviour",
                table: "ServiceRequests");
        }
    }
}
