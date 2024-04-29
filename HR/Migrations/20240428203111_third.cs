using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_informationAttendencperMonth_Employees_Employeeid",
                table: "informationAttendencperMonth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_informationAttendencperMonth",
                table: "informationAttendencperMonth");

            migrationBuilder.RenameTable(
                name: "informationAttendencperMonth",
                newName: "AttendencperMonths");

            migrationBuilder.RenameColumn(
                name: "Employeeid",
                table: "AttendencperMonths",
                newName: "idemp");

            migrationBuilder.RenameIndex(
                name: "IX_informationAttendencperMonth_Employeeid",
                table: "AttendencperMonths",
                newName: "IX_AttendencperMonths_idemp");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendencperMonths",
                table: "AttendencperMonths",
                column: "Monthofyear");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendencperMonths_Employees_idemp",
                table: "AttendencperMonths",
                column: "idemp",
                principalTable: "Employees",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendencperMonths_Employees_idemp",
                table: "AttendencperMonths");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendencperMonths",
                table: "AttendencperMonths");

            migrationBuilder.RenameTable(
                name: "AttendencperMonths",
                newName: "informationAttendencperMonth");

            migrationBuilder.RenameColumn(
                name: "idemp",
                table: "informationAttendencperMonth",
                newName: "Employeeid");

            migrationBuilder.RenameIndex(
                name: "IX_AttendencperMonths_idemp",
                table: "informationAttendencperMonth",
                newName: "IX_informationAttendencperMonth_Employeeid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_informationAttendencperMonth",
                table: "informationAttendencperMonth",
                column: "Monthofyear");

            migrationBuilder.AddForeignKey(
                name: "FK_informationAttendencperMonth_Employees_Employeeid",
                table: "informationAttendencperMonth",
                column: "Employeeid",
                principalTable: "Employees",
                principalColumn: "id");
        }
    }
}
