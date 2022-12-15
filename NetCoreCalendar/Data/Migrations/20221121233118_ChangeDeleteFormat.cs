using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreCalendar.Data.Migrations
{
    public partial class ChangeDeleteFormat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
            name: "FK_Lessons_Students_StudentId",
            table: "Lessons");
            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Students_StudentId",
                table: "Lessons",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
            name: "FK_Lessons_Students_StudentId",
            table: "Lessons");
            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Students_StudentId",
                table: "Lessons",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
