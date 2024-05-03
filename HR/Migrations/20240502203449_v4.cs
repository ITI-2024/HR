using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Holidays",
                table: "Holidays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendencpermonth",
                table: "Attendencpermonth");

            migrationBuilder.AlterColumn<string>(
                name: "dayName",
                table: "Holidays",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "Holidays",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "Attendencpermonth",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Holidays",
                table: "Holidays",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendencpermonth",
                table: "Attendencpermonth",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Holidays",
                table: "Holidays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendencpermonth",
                table: "Attendencpermonth");

            migrationBuilder.DropColumn(
                name: "id",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "id",
                table: "Attendencpermonth");

            migrationBuilder.AlterColumn<string>(
                name: "dayName",
                table: "Holidays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Holidays",
                table: "Holidays",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendencpermonth",
                table: "Attendencpermonth",
                column: "Monthofyear");
        }
    }
}
