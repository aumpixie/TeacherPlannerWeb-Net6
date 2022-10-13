using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreCalendar.Data.Migrations
{
    public partial class AddedUserIdToLesson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestingUserId",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                column: "ConcurrencyStamp",
                value: "6f536cbc-851c-4e02-9dc6-0fd182ac6cb5");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestingUserId",
                table: "Lessons");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                column: "ConcurrencyStamp",
                value: "845aca78-4079-4fc1-b91e-dbc478588839");
        }
    }
}
