using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class addobject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleNamepermission");

            migrationBuilder.RenameColumn(
                name: "read",
                table: "Permissions",
                newName: "view");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_roleid",
                table: "Permissions",
                column: "roleid");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_roleid",
                table: "Permissions",
                column: "roleid",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_roleid",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_roleid",
                table: "Permissions");

            migrationBuilder.RenameColumn(
                name: "view",
                table: "Permissions",
                newName: "read");

            migrationBuilder.CreateTable(
                name: "RoleNamepermission",
                columns: table => new
                {
                    Permissionsid = table.Column<int>(type: "int", nullable: false),
                    roleNameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleNamepermission", x => new { x.Permissionsid, x.roleNameId });
                    table.ForeignKey(
                        name: "FK_RoleNamepermission_Permissions_Permissionsid",
                        column: x => x.Permissionsid,
                        principalTable: "Permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleNamepermission_Roles_roleNameId",
                        column: x => x.roleNameId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleNamepermission_roleNameId",
                table: "RoleNamepermission",
                column: "roleNameId");
        }
    }
}
