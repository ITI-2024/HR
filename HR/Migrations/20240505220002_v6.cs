using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Roles_roleId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "roleId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Roles_roleId",
                table: "AspNetUsers",
                column: "roleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Roles_roleId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "roleId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Roles_roleId",
                table: "AspNetUsers",
                column: "roleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
