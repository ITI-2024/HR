using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class addTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "idDept",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendenceEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dayDate = table.Column<DateOnly>(type: "date", nullable: false),
                    arrivingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    leavingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    idemp = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendenceEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendenceEmployees_Employees_idemp",
                        column: x => x.idemp,
                        principalTable: "Employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    dayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HolidayName = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "PublicSettings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    extraHours = table.Column<int>(type: "int", nullable: false),
                    deductionHours = table.Column<int>(type: "int", nullable: false),
                    firstWeekend = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    secondWeekend = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicSettings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_idDept",
                table: "Employees",
                column: "idDept");

            migrationBuilder.CreateIndex(
                name: "IX_AttendenceEmployees_idemp",
                table: "AttendenceEmployees",
                column: "idemp");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_idDept",
                table: "Employees",
                column: "idDept",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_idDept",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "AttendenceEmployees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "PublicSettings");

            migrationBuilder.DropIndex(
                name: "IX_Employees_idDept",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "idDept",
                table: "Employees");
        }
    }
}
