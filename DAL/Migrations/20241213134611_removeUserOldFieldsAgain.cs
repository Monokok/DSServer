using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class removeUserOldFieldsAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "A_hours",
                table: "dsusers");

            migrationBuilder.DropColumn(
                name: "B_hours",
                table: "dsusers");

            migrationBuilder.DropColumn(
                name: "C_hours",
                table: "dsusers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "A_hours",
                table: "dsusers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "B_hours",
                table: "dsusers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "C_hours",
                table: "dsusers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
