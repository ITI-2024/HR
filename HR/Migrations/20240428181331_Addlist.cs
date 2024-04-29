using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class Addlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "informationAttendencperMonth",
                columns: table => new
                {
                    Monthofyear = table.Column<DateOnly>(type: "date", nullable: false),
                    Employeeid = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_informationAttendencperMonth", x => x.Monthofyear);
                    table.ForeignKey(
                        name: "FK_informationAttendencperMonth_Employees_Employeeid",
                        column: x => x.Employeeid,
                        principalTable: "Employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_informationAttendencperMonth_Employeeid",
                table: "informationAttendencperMonth",
                column: "Employeeid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "informationAttendencperMonth");
        }
    }
}
