using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class th : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendencperMonths");

            migrationBuilder.CreateTable(
                name: "Attendencpermonth",
                columns: table => new
                {
                    Monthofyear = table.Column<DateOnly>(type: "date", nullable: false),
                    idemp = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendencpermonth", x => x.Monthofyear);
                    table.ForeignKey(
                        name: "FK_Attendencpermonth_Employees_idemp",
                        column: x => x.idemp,
                        principalTable: "Employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendencpermonth_idemp",
                table: "Attendencpermonth",
                column: "idemp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendencpermonth");

            migrationBuilder.CreateTable(
                name: "AttendencperMonths",
                columns: table => new
                {
                    Monthofyear = table.Column<DateOnly>(type: "date", nullable: false),
                    idemp = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendencperMonths", x => x.Monthofyear);
                    table.ForeignKey(
                        name: "FK_AttendencperMonths_Employees_idemp",
                        column: x => x.idemp,
                        principalTable: "Employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendencperMonths_idemp",
                table: "AttendencperMonths",
                column: "idemp");
        }
    }
}
