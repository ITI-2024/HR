using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class addDeptToAttendence2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_AttendenceEmployees_idDept",
                table: "AttendenceEmployees",
                column: "idDept");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendenceEmployees_Departments_idDept",
                table: "AttendenceEmployees",
                column: "idDept",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendenceEmployees_Departments_idDept",
                table: "AttendenceEmployees");

            migrationBuilder.DropIndex(
                name: "IX_AttendenceEmployees_idDept",
                table: "AttendenceEmployees");

            migrationBuilder.AddColumn<int>(
                name: "departmentId",
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
    }
}
