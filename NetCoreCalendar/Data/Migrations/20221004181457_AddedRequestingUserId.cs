using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreCalendar.Data.Migrations
{
    public partial class AddedRequestingUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestingUserId",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                column: "ConcurrencyStamp",
                value: "845aca78-4079-4fc1-b91e-dbc478588839");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestingUserId",
                table: "Students");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                column: "ConcurrencyStamp",
                value: "0a41fa9a-8e93-4f7f-a677-526c6e281cf2");
        }
    }
}
