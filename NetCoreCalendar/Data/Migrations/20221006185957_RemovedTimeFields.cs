using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreCalendar.Data.Migrations
{
    public partial class RemovedTimeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Lessons",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Lessons",
                newName: "End");

            migrationBuilder.CreateTable(
                name: "StudentCreateVM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Goal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCreateVM", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                column: "ConcurrencyStamp",
                value: "fdd0ec85-3dc8-41b1-9146-3b6027ec938f");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentCreateVM");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Lessons",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Lessons",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                column: "ConcurrencyStamp",
                value: "dd312797-a9e6-4a00-8112-facb73c25fdb");
        }
    }
}
