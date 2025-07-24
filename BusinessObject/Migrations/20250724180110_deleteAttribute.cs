using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class deleteAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCertificates_Employee_ApprovedBy",
                table: "EmployeeCertificates");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeCertificates_ApprovedBy",
                table: "EmployeeCertificates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCertificates_ApprovedBy",
                table: "EmployeeCertificates",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCertificates_Employee_ApprovedBy",
                table: "EmployeeCertificates",
                column: "ApprovedBy",
                principalTable: "Employee",
                principalColumn: "EmployId");
        }
    }
}
