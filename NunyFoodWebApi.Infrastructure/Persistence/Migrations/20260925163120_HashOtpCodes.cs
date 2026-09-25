using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NunyFoodWebApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HashOtpCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "OtpCodes");

            migrationBuilder.AddColumn<string>(
                name: "CodeHash",
                table: "OtpCodes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeHash",
                table: "OtpCodes");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "OtpCodes",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");
        }
    }
}
