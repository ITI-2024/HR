using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class addrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "roleId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    read = table.Column<bool>(type: "bit", nullable: true),
                    create = table.Column<bool>(type: "bit", nullable: true),
                    update = table.Column<bool>(type: "bit", nullable: true),
                    delete = table.Column<bool>(type: "bit", nullable: true),
                    roleid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

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
                name: "IX_AspNetUsers_roleId",
                table: "AspNetUsers",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleNamepermission_roleNameId",
                table: "RoleNamepermission",
                column: "roleNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Roles_roleId",
                table: "AspNetUsers",
                column: "roleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Roles_roleId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "RoleNamepermission");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_roleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "roleId",
                table: "AspNetUsers");
        }
    }
}
