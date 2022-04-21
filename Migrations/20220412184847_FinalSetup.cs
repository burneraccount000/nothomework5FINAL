using Microsoft.EntityFrameworkCore.Migrations;

namespace Ramirez_Mackenzie_HW5.Migrations
{
    public partial class FinalSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "productType",
                table: "Products",
                newName: "ProductType");

            migrationBuilder.AddColumn<decimal>(
                name: "ExtendedPrice",
                table: "OrderDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtendedPrice",
                table: "OrderDetail");

            migrationBuilder.RenameColumn(
                name: "ProductType",
                table: "Products",
                newName: "productType");
        }
    }
}
