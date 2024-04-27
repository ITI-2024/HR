using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class addDeptToAttendence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "departmentId",
                table: "AttendenceEmployees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idDept",
                table: "AttendenceEmployees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendenceEmployees_departmentId",
                table: "AttendenceEmployees",
                column: "departmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendenceEmployees_Departments_departmentId",
                table: "AttendenceEmployees",
                column: "departmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendenceEmployees_Departments_departmentId",
                table: "AttendenceEmployees");

            migrationBuilder.DropIndex(
                name: "IX_AttendenceEmployees_departmentId",
                table: "AttendenceEmployees");

            migrationBuilder.DropColumn(
                name: "departmentId",
                table: "AttendenceEmployees");

            migrationBuilder.DropColumn(
                name: "idDept",
                table: "AttendenceEmployees");
        }
    }
}
