using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiEventos.Migrations
{
    /// <inheritdoc />
    public partial class AddCoupons2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Coupons_CouponsId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CouponsId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CouponsId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "EventsId",
                table: "Coupons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_EventsId",
                table: "Coupons",
                column: "EventsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Events_EventsId",
                table: "Coupons",
                column: "EventsId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Events_EventsId",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_EventsId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "EventsId",
                table: "Coupons");

            migrationBuilder.AddColumn<int>(
                name: "CouponsId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_CouponsId",
                table: "Events",
                column: "CouponsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Coupons_CouponsId",
                table: "Events",
                column: "CouponsId",
                principalTable: "Coupons",
                principalColumn: "Id");
        }
    }
}
