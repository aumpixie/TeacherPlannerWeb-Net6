using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreCalendar.Data.Migrations
{
    public partial class AddedDefaultUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e71edcef-91ae-468c-98a2-a3d88d3fd01b", "0a41fa9a-8e93-4f7f-a677-526c6e281cf2", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e71edcef-91ae-468c-98a2-a3d88d3fd01b");
        }
    }
}
