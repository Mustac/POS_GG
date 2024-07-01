using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS_OS_GG.Migrations
{
    /// <inheritdoc />
    public partial class addedMeasurementAndQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Measurement",
                table: "Products");

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "OrderProducts",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Measurement",
                table: "OrderProducts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Measurement",
                table: "OrderProducts");

            migrationBuilder.AddColumn<int>(
                name: "Measurement",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderProducts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
