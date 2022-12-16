using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockDoor.Data.Migrations.SqlServer
{
    public partial class Latest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestHeaders_ServiceRequests_ServiceRequestID",
                table: "RequestHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseHeaders_RequestResponse_RequestResponseID",
                table: "ResponseHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHeaders_Microservices_MicroserviceID",
                table: "ServiceHeaders");

            migrationBuilder.AlterColumn<int>(
                name: "MicroserviceID",
                table: "ServiceHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequestResponseID",
                table: "ResponseHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceRequestID",
                table: "RequestHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeadersMode",
                table: "Microservices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHeaders_ServiceRequests_ServiceRequestID",
                table: "RequestHeaders",
                column: "ServiceRequestID",
                principalTable: "ServiceRequests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseHeaders_RequestResponse_RequestResponseID",
                table: "ResponseHeaders",
                column: "RequestResponseID",
                principalTable: "RequestResponse",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHeaders_Microservices_MicroserviceID",
                table: "ServiceHeaders",
                column: "MicroserviceID",
                principalTable: "Microservices",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestHeaders_ServiceRequests_ServiceRequestID",
                table: "RequestHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseHeaders_RequestResponse_RequestResponseID",
                table: "ResponseHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHeaders_Microservices_MicroserviceID",
                table: "ServiceHeaders");

            migrationBuilder.DropColumn(
                name: "HeadersMode",
                table: "Microservices");

            migrationBuilder.AlterColumn<int>(
                name: "MicroserviceID",
                table: "ServiceHeaders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RequestResponseID",
                table: "ResponseHeaders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceRequestID",
                table: "RequestHeaders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHeaders_ServiceRequests_ServiceRequestID",
                table: "RequestHeaders",
                column: "ServiceRequestID",
                principalTable: "ServiceRequests",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseHeaders_RequestResponse_RequestResponseID",
                table: "ResponseHeaders",
                column: "RequestResponseID",
                principalTable: "RequestResponse",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHeaders_Microservices_MicroserviceID",
                table: "ServiceHeaders",
                column: "MicroserviceID",
                principalTable: "Microservices",
                principalColumn: "ID");
        }
    }
}
