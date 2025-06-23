using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldDobToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Dob",
                table: "Employee",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Dob",
                table: "Employee",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
