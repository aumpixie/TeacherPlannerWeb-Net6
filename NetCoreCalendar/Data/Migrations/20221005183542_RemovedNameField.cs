using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreCalendar.Data.Migrations
{
    public partial class RemovedNameField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Lessons");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                column: "ConcurrencyStamp",
                value: "dd312797-a9e6-4a00-8112-facb73c25fdb");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                column: "ConcurrencyStamp",
                value: "3421d231-0f5d-4973-8431-7bb997979074");
        }
    }
}
