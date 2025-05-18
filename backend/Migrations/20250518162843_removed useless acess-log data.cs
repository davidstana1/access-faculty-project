using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class removeduselessacesslogdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWithinSchedule",
                table: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "OverrideUserId",
                table: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "WasOverridden",
                table: "AccessLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWithinSchedule",
                table: "AccessLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OverrideUserId",
                table: "AccessLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "WasOverridden",
                table: "AccessLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
