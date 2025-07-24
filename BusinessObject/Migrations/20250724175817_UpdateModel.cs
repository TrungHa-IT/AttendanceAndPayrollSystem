using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCertificateImage_EmployeeCertificates_EmployeeCertificateId",
                table: "EmployeeCertificateImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeCertificateImage",
                table: "EmployeeCertificateImage");

            migrationBuilder.RenameTable(
                name: "EmployeeCertificateImage",
                newName: "EmployeeCertificateImages");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeCertificateImage_EmployeeCertificateId",
                table: "EmployeeCertificateImages",
                newName: "IX_EmployeeCertificateImages_EmployeeCertificateId");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "EmployeeCertificates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EmployeeCertificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeCertificateImages",
                table: "EmployeeCertificateImages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCertificates_ApprovedBy",
                table: "EmployeeCertificates",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCertificateImages_EmployeeCertificates_EmployeeCertificateId",
                table: "EmployeeCertificateImages",
                column: "EmployeeCertificateId",
                principalTable: "EmployeeCertificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCertificates_Employee_ApprovedBy",
                table: "EmployeeCertificates",
                column: "ApprovedBy",
                principalTable: "Employee",
                principalColumn: "EmployId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCertificateImages_EmployeeCertificates_EmployeeCertificateId",
                table: "EmployeeCertificateImages");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCertificates_Employee_ApprovedBy",
                table: "EmployeeCertificates");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeCertificates_ApprovedBy",
                table: "EmployeeCertificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeCertificateImages",
                table: "EmployeeCertificateImages");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "EmployeeCertificates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmployeeCertificates");

            migrationBuilder.RenameTable(
                name: "EmployeeCertificateImages",
                newName: "EmployeeCertificateImage");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeCertificateImages_EmployeeCertificateId",
                table: "EmployeeCertificateImage",
                newName: "IX_EmployeeCertificateImage_EmployeeCertificateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeCertificateImage",
                table: "EmployeeCertificateImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCertificateImage_EmployeeCertificates_EmployeeCertificateId",
                table: "EmployeeCertificateImage",
                column: "EmployeeCertificateId",
                principalTable: "EmployeeCertificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
