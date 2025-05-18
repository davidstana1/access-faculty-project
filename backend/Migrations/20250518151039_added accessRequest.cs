using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class addedaccessRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GateStatuses_Employees_LastAccessEmployeeId",
                table: "GateStatuses");

            migrationBuilder.DropIndex(
                name: "IX_GateStatuses_LastAccessEmployeeId",
                table: "GateStatuses");

            migrationBuilder.DropColumn(
                name: "LastAccessEmployeeId",
                table: "GateStatuses");

            migrationBuilder.DropColumn(
                name: "LastDirection",
                table: "GateStatuses");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "GateStatuses",
                newName: "LastOperationTime");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "GateStatuses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsOperational",
                table: "GateStatuses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastOperation",
                table: "GateStatuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AccessRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessRequests");

            migrationBuilder.DropColumn(
                name: "IsOperational",
                table: "GateStatuses");

            migrationBuilder.DropColumn(
                name: "LastOperation",
                table: "GateStatuses");

            migrationBuilder.RenameColumn(
                name: "LastOperationTime",
                table: "GateStatuses",
                newName: "LastUpdated");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "GateStatuses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "LastAccessEmployeeId",
                table: "GateStatuses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastDirection",
                table: "GateStatuses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GateStatuses_LastAccessEmployeeId",
                table: "GateStatuses",
                column: "LastAccessEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GateStatuses_Employees_LastAccessEmployeeId",
                table: "GateStatuses",
                column: "LastAccessEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
