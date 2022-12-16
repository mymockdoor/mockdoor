using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mockdoor.Data.Sqlite.Migrations
{
    public partial class RenameHeaderName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HeaderName",
                table: "ServiceHeaders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "HeaderName",
                table: "ResponseHeaders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "HeaderName",
                table: "RequestHeaders",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ServiceHeaders",
                newName: "HeaderName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ResponseHeaders",
                newName: "HeaderName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "RequestHeaders",
                newName: "HeaderName");
        }
    }
}
