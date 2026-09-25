using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NunyFoodWebApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameDeliveryProofPaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignatureUrl",
                table: "Deliveries",
                newName: "SignaturePath");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Deliveries",
                newName: "PhotoPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignaturePath",
                table: "Deliveries",
                newName: "SignatureUrl");

            migrationBuilder.RenameColumn(
                name: "PhotoPath",
                table: "Deliveries",
                newName: "PhotoUrl");
        }
    }
}
