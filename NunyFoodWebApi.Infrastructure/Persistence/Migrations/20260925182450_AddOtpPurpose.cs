using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NunyFoodWebApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpPurpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpCodes_Email_Role",
                table: "OtpCodes");

            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "OtpCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_Email_Role_Purpose",
                table: "OtpCodes",
                columns: new[] { "Email", "Role", "Purpose" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpCodes_Email_Role_Purpose",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "OtpCodes");

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_Email_Role",
                table: "OtpCodes",
                columns: new[] { "Email", "Role" });
        }
    }
}
