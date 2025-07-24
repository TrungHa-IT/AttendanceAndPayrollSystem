using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBonus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCertificates_CertificateBonusRate_CertificateBonusRateId",
                table: "EmployeeCertificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CertificateBonusRate",
                table: "CertificateBonusRate");

            migrationBuilder.RenameTable(
                name: "CertificateBonusRate",
                newName: "CertificateBonusRates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CertificateBonusRates",
                table: "CertificateBonusRates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCertificates_CertificateBonusRates_CertificateBonusRateId",
                table: "EmployeeCertificates",
                column: "CertificateBonusRateId",
                principalTable: "CertificateBonusRates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCertificates_CertificateBonusRates_CertificateBonusRateId",
                table: "EmployeeCertificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CertificateBonusRates",
                table: "CertificateBonusRates");

            migrationBuilder.RenameTable(
                name: "CertificateBonusRates",
                newName: "CertificateBonusRate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CertificateBonusRate",
                table: "CertificateBonusRate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCertificates_CertificateBonusRate_CertificateBonusRateId",
                table: "EmployeeCertificates",
                column: "CertificateBonusRateId",
                principalTable: "CertificateBonusRate",
                principalColumn: "Id");
        }
    }
}
