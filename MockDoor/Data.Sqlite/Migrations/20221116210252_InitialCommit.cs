using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mockdoor.Data.Sqlite.Migrations
{
    public partial class InitialCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SimulateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceGroups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TenantID = table.Column<int>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultHealthCheckUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SimulateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
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
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ServiceGroupID = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetUrl = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    FakeDelay = table.Column<int>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProxyMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    SimulateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RandomiseMockResult = table.Column<bool>(type: "INTEGER", nullable: false),
                    PassThroughTenant = table.Column<bool>(type: "INTEGER", nullable: false),
                    HeadersMode = table.Column<int>(type: "INTEGER", nullable: false)
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
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Incoming = table.Column<bool>(type: "INTEGER", nullable: false),
                    Outgoing = table.Column<bool>(type: "INTEGER", nullable: false),
                    MicroserviceID = table.Column<int>(type: "INTEGER", nullable: false),
                    HeaderName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceHeaders_Microservices_MicroserviceID",
                        column: x => x.MicroserviceID,
                        principalTable: "Microservices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FromUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ExactUrlMatch = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpectAuthHeader = table.Column<bool>(type: "INTEGER", nullable: false),
                    MockBehaviour = table.Column<int>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    RestType = table.Column<int>(type: "INTEGER", nullable: false),
                    SimulateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FromBody = table.Column<string>(type: "TEXT", nullable: true),
                    TTLTicks = table.Column<long>(type: "INTEGER", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MicroserviceID = table.Column<int>(type: "INTEGER", nullable: false)
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
                name: "MockResponses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Code = table.Column<int>(type: "INTEGER", nullable: false),
                    Encoding = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    ServiceRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    Checksum = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Latency = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "QueryParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryParameters_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestHeaders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    ServiceRequestID = table.Column<int>(type: "INTEGER", nullable: false),
                    HeaderName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RequestHeaders_ServiceRequests_ServiceRequestID",
                        column: x => x.ServiceRequestID,
                        principalTable: "ServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponseHeaders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    MockResponseID = table.Column<int>(type: "INTEGER", nullable: false),
                    HeaderName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ResponseHeaders_MockResponses_MockResponseID",
                        column: x => x.MockResponseID,
                        principalTable: "MockResponses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Microservices_ServiceGroupID",
                table: "Microservices",
                column: "ServiceGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_MockResponses_ServiceRequestId",
                table: "MockResponses",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryParameters_ServiceRequestId",
                table: "QueryParameters",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHeaders_ServiceRequestID",
                table: "RequestHeaders",
                column: "ServiceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseHeaders_MockResponseID",
                table: "ResponseHeaders",
                column: "MockResponseID");

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
                name: "QueryParameters");

            migrationBuilder.DropTable(
                name: "RequestHeaders");

            migrationBuilder.DropTable(
                name: "ResponseHeaders");

            migrationBuilder.DropTable(
                name: "ServiceHeaders");

            migrationBuilder.DropTable(
                name: "MockResponses");

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
