using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS_OS_GG.Migrations
{
    /// <inheritdoc />
    public partial class setNullProductUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_UserRegistratedId",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "UserRegistratedId",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_UserRegistratedId",
                table: "Products",
                column: "UserRegistratedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_UserRegistratedId",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "UserRegistratedId",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_UserRegistratedId",
                table: "Products",
                column: "UserRegistratedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
