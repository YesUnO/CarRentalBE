using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class stripeSubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentCards_PaymentCardId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "PaymentCards");

            migrationBuilder.CreateTable(
                name: "StripeSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<int>(type: "integer", nullable: false),
                    StripeSubscriptionId = table.Column<string>(type: "text", nullable: true),
                    CheckoutSessionReferenceId = table.Column<string>(type: "text", nullable: true),
                    StripeSubscriptionStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeSubscriptions_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_StripeSubscriptions_ApplicationUserId",
                table: "StripeSubscriptions",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_StripeSubscriptions_PaymentCardId",
                table: "Payments",
                column: "PaymentCardId",
                principalTable: "StripeSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_StripeSubscriptions_PaymentCardId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "StripeSubscriptions");

            migrationBuilder.CreateTable(
                name: "PaymentCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCards", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b5141f7-3aed-4cf9-a51d-4ad671703e1f",
                column: "ConcurrencyStamp",
                value: "586849e6-449b-4e93-98e6-c1cfdcd32ae0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                column: "ConcurrencyStamp",
                value: "1fde0367-fa39-4202-b73f-ac7ca3c440d9");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4ca025a0-0539-4e9b-a978-b7409c03f216", "AQAAAAEAACcQAAAAEOTcnHM0+Sf0yi9wbbTgNkdan/IR5AXpP8jbJLeppKHqWIUN4uN7w99NRNKtZIzd/g==", "8f9d9f11-74ff-429d-9de0-9670a15f19ad" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentCards_PaymentCardId",
                table: "Payments",
                column: "PaymentCardId",
                principalTable: "PaymentCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
