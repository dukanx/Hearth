using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hearth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HouseholdDualJoinCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JoinCode",
                table: "Households",
                newName: "ChildJoinCode");

            migrationBuilder.RenameIndex(
                name: "IX_Households_JoinCode",
                table: "Households",
                newName: "IX_Households_ChildJoinCode");

            migrationBuilder.AddColumn<string>(
                name: "AdultJoinCode",
                table: "Households",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Households_AdultJoinCode",
                table: "Households",
                column: "AdultJoinCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Households_AdultJoinCode",
                table: "Households");

            migrationBuilder.DropColumn(
                name: "AdultJoinCode",
                table: "Households");

            migrationBuilder.RenameColumn(
                name: "ChildJoinCode",
                table: "Households",
                newName: "JoinCode");

            migrationBuilder.RenameIndex(
                name: "IX_Households_ChildJoinCode",
                table: "Households",
                newName: "IX_Households_JoinCode");
        }
    }
}
