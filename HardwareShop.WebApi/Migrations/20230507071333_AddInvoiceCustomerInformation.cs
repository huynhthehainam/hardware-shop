using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareShop.WebApi.Migrations
{
    public partial class AddInvoiceCustomerInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerInformation",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerInformation",
                table: "Invoices");
        }
    }
}
