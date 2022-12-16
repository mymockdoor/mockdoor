using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockDoor.Data.Migrations.SqlServer
{
    public partial class RenameMockRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseHeaders_RequestResponse_RequestResponseID",
                table: "ResponseHeaders");

            migrationBuilder.DropTable(
                name: "RequestResponse");

            migrationBuilder.RenameColumn(
                name: "SimilateTime",
                table: "ServiceRequests",
                newName: "SimulateTime");

            migrationBuilder.RenameColumn(
                name: "RequestResponseID",
                table: "ResponseHeaders",
                newName: "MockResponseID");

            migrationBuilder.RenameIndex(
                name: "IX_ResponseHeaders_RequestResponseID",
                table: "ResponseHeaders",
                newName: "IX_ResponseHeaders_MockResponseID");

            migrationBuilder.CreateTable(
                name: "MockResponses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Encoding = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    Checksum = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Latency = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockResponses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MockResponses_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MockResponses_ServiceRequestId",
                table: "MockResponses",
                column: "ServiceRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseHeaders_MockResponses_MockResponseID",
                table: "ResponseHeaders",
                column: "MockResponseID",
                principalTable: "MockResponses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseHeaders_MockResponses_MockResponseID",
                table: "ResponseHeaders");

            migrationBuilder.DropTable(
                name: "MockResponses");

            migrationBuilder.RenameColumn(
                name: "SimulateTime",
                table: "ServiceRequests",
                newName: "SimilateTime");

            migrationBuilder.RenameColumn(
                name: "MockResponseID",
                table: "ResponseHeaders",
                newName: "RequestResponseID");

            migrationBuilder.RenameIndex(
                name: "IX_ResponseHeaders_MockResponseID",
                table: "ResponseHeaders",
                newName: "IX_ResponseHeaders_RequestResponseID");

            migrationBuilder.CreateTable(
                name: "RequestResponse",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Checksum = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Encoding = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 100)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestResponse", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RequestResponse_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponse_ServiceRequestId",
                table: "RequestResponse",
                column: "ServiceRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseHeaders_RequestResponse_RequestResponseID",
                table: "ResponseHeaders",
                column: "RequestResponseID",
                principalTable: "RequestResponse",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
