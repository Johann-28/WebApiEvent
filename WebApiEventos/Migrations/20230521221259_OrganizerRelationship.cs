using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiEventos.Migrations
{
    /// <inheritdoc />
    public partial class OrganizerRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizersId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrgnaizerId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_OrganizersId",
                table: "Comments",
                column: "OrganizersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Organizers_OrganizersId",
                table: "Comments",
                column: "OrganizersId",
                principalTable: "Organizers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Organizers_OrganizersId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_OrganizersId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "OrganizersId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "OrgnaizerId",
                table: "Comments");
        }
    }
}
