using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiEventos.Migrations
{
    /// <inheritdoc />
    public partial class UserOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsersId",
                table: "Organizers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizers_UsersId",
                table: "Organizers",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizers_Users_UsersId",
                table: "Organizers",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizers_Users_UsersId",
                table: "Organizers");

            migrationBuilder.DropIndex(
                name: "IX_Organizers_UsersId",
                table: "Organizers");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Organizers");
        }
    }
}
