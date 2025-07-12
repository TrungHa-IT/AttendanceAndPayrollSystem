using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCheckInDefaultToOvertimeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCheckIn",
                table: "OvertimeRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Salary",
                table: "Employee",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCheckIn",
                table: "OvertimeRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "Salary",
                table: "Employee",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");
        }
    }
}
