using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_roleid",
                table: "Permissions");

            migrationBuilder.AlterColumn<int>(
                name: "roleid",
                table: "Permissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_roleid",
                table: "Permissions",
                column: "roleid",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_roleid",
                table: "Permissions");

            migrationBuilder.AlterColumn<int>(
                name: "roleid",
                table: "Permissions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_roleid",
                table: "Permissions",
                column: "roleid",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
