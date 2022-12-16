using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockDoor.Data.Migrations.SqlServer
{
    public partial class AddSimulateTimeToEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SimulateTime",
                table: "Tenants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SimulateTime",
                table: "ServiceGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SimulateTime",
                table: "Microservices",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SimulateTime",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SimulateTime",
                table: "ServiceGroups");

            migrationBuilder.DropColumn(
                name: "SimulateTime",
                table: "Microservices");
        }
    }
}
