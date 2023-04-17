using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class stripeInvoiceDraft : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_StripeSubscriptions_PaymentCardId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentCardId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CanceledAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BasicRentalPrice",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "PaymentCardId",
                table: "Payments",
                newName: "StripeInvoiceStatus");

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "StripeSubscriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AmountDue",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AmountPaid",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "StripeInvoiceId",
                table: "Payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b5141f7-3aed-4cf9-a51d-4ad671703e1f",
                column: "ConcurrencyStamp",
                value: "a31e6a47-bb7d-4cd2-b91f-5fe0c4cd88ee");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                column: "ConcurrencyStamp",
                value: "e520e891-c5ae-4351-aff0-8e05d1d2ddca");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5609a95b-9c48-498c-9345-a44f417d7a17", "AQAAAAEAACcQAAAAENDQuYHysDO249Nwh0snPSWO/lrqmeNQm4H2lejMsLeEdBBOBJ18d32eTFeoFkdSog==", "5a00b542-9e97-4ce0-ba0a-253d69f40813" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "StripeSubscriptions");

            migrationBuilder.DropColumn(
                name: "AmountDue",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StripeInvoiceId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "StripeInvoiceStatus",
                table: "Payments",
                newName: "PaymentCardId");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CanceledAt",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BasicRentalPrice",
                table: "Cars",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b5141f7-3aed-4cf9-a51d-4ad671703e1f",
                column: "ConcurrencyStamp",
                value: "0d1c2b66-48b6-416d-b974-8facd4485c12");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                column: "ConcurrencyStamp",
                value: "47a6dc52-2990-4e9e-baa8-bd3673fc8977");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d907fdfc-c074-4c58-b166-21c53d5cb081", "AQAAAAEAACcQAAAAELfrtTIb6ti6SHC5nXaNZf511hQQRR7aQSqDQJJozE88XFajPydUVf6meiu7HYdJgw==", "dd7ad5d3-b2f6-4fb8-b62a-a69d48f244ab" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentCardId",
                table: "Payments",
                column: "PaymentCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_StripeSubscriptions_PaymentCardId",
                table: "Payments",
                column: "PaymentCardId",
                principalTable: "StripeSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
