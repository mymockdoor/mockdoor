using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MockDoor.Data.Migrations.SqlServer
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceGroups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenantID = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    DefaultHealthCheckUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceGroups_Tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenants",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Microservices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ServiceGroupID = table.Column<int>(type: "int", nullable: false),
                    TargetUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FakeDelay = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    MockResult = table.Column<bool>(type: "bit", nullable: false),
                    RandomiseMockResult = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Microservices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Microservices_ServiceGroups_ServiceGroupID",
                        column: x => x.ServiceGroupID,
                        principalTable: "ServiceGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceHeaders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Incoming = table.Column<bool>(type: "bit", nullable: false),
                    Outgoing = table.Column<bool>(type: "bit", nullable: false),
                    MicroserviceID = table.Column<int>(type: "int", nullable: true),
                    HeaderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceHeaders_Microservices_MicroserviceID",
                        column: x => x.MicroserviceID,
                        principalTable: "Microservices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExactUrlMatch = table.Column<bool>(type: "bit", nullable: false),
                    ExpectAuthHeader = table.Column<bool>(type: "bit", nullable: false),
                    RestType = table.Column<int>(type: "int", nullable: false),
                    FromBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TTLTicks = table.Column<long>(type: "bigint", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    MicroserviceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Microservices_MicroserviceID",
                        column: x => x.MicroserviceID,
                        principalTable: "Microservices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestHeaders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceRequestID = table.Column<int>(type: "int", nullable: true),
                    HeaderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RequestHeaders_ServiceRequests_ServiceRequestID",
                        column: x => x.ServiceRequestID,
                        principalTable: "ServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestResponse",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Encoding = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    Checksum = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ResponseHeaders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestResponseID = table.Column<int>(type: "int", nullable: true),
                    HeaderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ResponseHeaders_RequestResponse_RequestResponseID",
                        column: x => x.RequestResponseID,
                        principalTable: "RequestResponse",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Microservices_ServiceGroupID",
                table: "Microservices",
                column: "ServiceGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHeaders_ServiceRequestID",
                table: "RequestHeaders",
                column: "ServiceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponse_ServiceRequestId",
                table: "RequestResponse",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseHeaders_RequestResponseID",
                table: "ResponseHeaders",
                column: "RequestResponseID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceGroups_TenantID",
                table: "ServiceGroups",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHeaders_MicroserviceID",
                table: "ServiceHeaders",
                column: "MicroserviceID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_MicroserviceID",
                table: "ServiceRequests",
                column: "MicroserviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Path",
                table: "Tenants",
                column: "Path",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestHeaders");

            migrationBuilder.DropTable(
                name: "ResponseHeaders");

            migrationBuilder.DropTable(
                name: "ServiceHeaders");

            migrationBuilder.DropTable(
                name: "RequestResponse");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "Microservices");

            migrationBuilder.DropTable(
                name: "ServiceGroups");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
