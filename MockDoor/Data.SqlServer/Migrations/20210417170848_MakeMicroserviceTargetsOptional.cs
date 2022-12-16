using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MockDoor.Data.Migrations.SqlServer
{
    public partial class MakeMicroserviceTargetsOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MockResult",
                table: "Microservices",
                newName: "ProxyMode");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUrl",
                table: "Microservices",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProxyMode",
                table: "Microservices",
                newName: "MockResult");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUrl",
                table: "Microservices",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);
        }
    }
}
