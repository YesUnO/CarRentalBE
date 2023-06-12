using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarInsurances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarInsurances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarSpecifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Length = table.Column<decimal>(type: "numeric", nullable: false),
                    Width = table.Column<decimal>(type: "numeric", nullable: false),
                    Height = table.Column<decimal>(type: "numeric", nullable: false),
                    TrunkLength = table.Column<decimal>(type: "numeric", nullable: false),
                    TrunkWidth = table.Column<decimal>(type: "numeric", nullable: false),
                    TrunkHeight = table.Column<decimal>(type: "numeric", nullable: false),
                    TrunkVolume = table.Column<decimal>(type: "numeric", nullable: false),
                    LoadCapacity = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberOfSeats = table.Column<int>(type: "integer", nullable: false),
                    ManufacturedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ManufacturedIn = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    DriversLicenseId = table.Column<int>(type: "integer", nullable: true),
                    IdentificationCardId = table.Column<int>(type: "integer", nullable: true),
                    Approved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StripeSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<int>(type: "integer", nullable: false),
                    StripeSubscriptionId = table.Column<string>(type: "text", nullable: true),
                    StripeCustomerId = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "CarDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CarDocumentType = table.Column<int>(type: "integer", nullable: false),
                    FrontSideImageId = table.Column<int>(type: "integer", nullable: false),
                    BackSideImageId = table.Column<int>(type: "integer", nullable: false),
                    DocNr = table.Column<string>(type: "text", nullable: true),
                    ValidTill = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CarSpecificationId = table.Column<int>(type: "integer", nullable: true),
                    ProfilePicId = table.Column<int>(type: "integer", nullable: true),
                    STKId = table.Column<int>(type: "integer", nullable: true),
                    TechnicLicenseId = table.Column<int>(type: "integer", nullable: true),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MileageAtPurchase = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentMileage = table.Column<decimal>(type: "numeric", nullable: false),
                    CarInsuranceId = table.Column<int>(type: "integer", nullable: true),
                    CarState = table.Column<int>(type: "integer", nullable: false),
                    CarServiceState = table.Column<int>(type: "integer", nullable: false),
                    StripePriceId = table.Column<string>(type: "text", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cars_CarDocuments_STKId",
                        column: x => x.STKId,
                        principalTable: "CarDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cars_CarDocuments_TechnicLicenseId",
                        column: x => x.TechnicLicenseId,
                        principalTable: "CarDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cars_CarInsurances_CarInsuranceId",
                        column: x => x.CarInsuranceId,
                        principalTable: "CarInsurances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cars_CarSpecifications_CarSpecificationId",
                        column: x => x.CarSpecificationId,
                        principalTable: "CarSpecifications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Accidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: true),
                    CarId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accidents_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    CarId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccidentId = table.Column<int>(type: "integer", nullable: true),
                    Distance = table.Column<decimal>(type: "numeric", nullable: false),
                    HasBeenPayed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Accidents_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "Accidents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_ApplicationUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PDFs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RelativePath = table.Column<string>(type: "text", nullable: false),
                    CarId = table.Column<int>(type: "integer", nullable: true),
                    CarInsuranceId = table.Column<int>(type: "integer", nullable: true),
                    AccidentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDFs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDFs_Accidents_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "Accidents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PDFs_CarInsurances_CarInsuranceId",
                        column: x => x.CarInsuranceId,
                        principalTable: "CarInsurances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PDFs_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RelativePath = table.Column<string>(type: "text", nullable: false),
                    ImageType = table.Column<string>(type: "text", nullable: false),
                    AccidentId = table.Column<int>(type: "integer", nullable: true),
                    CarImageType = table.Column<int>(type: "integer", nullable: true),
                    OrderId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Accidents_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "Accidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StripeInvoiceId = table.Column<string>(type: "text", nullable: true),
                    AmountPaid = table.Column<long>(type: "bigint", nullable: false),
                    AmountDue = table.Column<long>(type: "bigint", nullable: false),
                    StripeInvoiceStatus = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserDocumentType = table.Column<int>(type: "integer", nullable: false),
                    FrontSideImageId = table.Column<int>(type: "integer", nullable: true),
                    BackSideImageId = table.Column<int>(type: "integer", nullable: true),
                    Checked = table.Column<bool>(type: "boolean", nullable: false),
                    DocNr = table.Column<string>(type: "text", nullable: true),
                    ValidTill = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDocuments_Images_BackSideImageId",
                        column: x => x.BackSideImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserDocuments_Images_FrontSideImageId",
                        column: x => x.FrontSideImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accidents_CarId",
                table: "Accidents",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Accidents_OrderId",
                table: "Accidents",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_DriversLicenseId",
                table: "ApplicationUsers",
                column: "DriversLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_IdentificationCardId",
                table: "ApplicationUsers",
                column: "IdentificationCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDocuments_BackSideImageId",
                table: "CarDocuments",
                column: "BackSideImageId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDocuments_FrontSideImageId",
                table: "CarDocuments",
                column: "FrontSideImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarInsuranceId",
                table: "Cars",
                column: "CarInsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarSpecificationId",
                table: "Cars",
                column: "CarSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_ProfilePicId",
                table: "Cars",
                column: "ProfilePicId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_STKId",
                table: "Cars",
                column: "STKId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_TechnicLicenseId",
                table: "Cars",
                column: "TechnicLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_AccidentId",
                table: "Images",
                column: "AccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_OrderId",
                table: "Images",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AccidentId",
                table: "Orders",
                column: "AccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CarId",
                table: "Orders",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PDFs_AccidentId",
                table: "PDFs",
                column: "AccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_PDFs_CarId",
                table: "PDFs",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_PDFs_CarInsuranceId",
                table: "PDFs",
                column: "CarInsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeSubscriptions_ApplicationUserId",
                table: "StripeSubscriptions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDocuments_BackSideImageId",
                table: "UserDocuments",
                column: "BackSideImageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDocuments_FrontSideImageId",
                table: "UserDocuments",
                column: "FrontSideImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_UserDocuments_DriversLicenseId",
                table: "ApplicationUsers",
                column: "DriversLicenseId",
                principalTable: "UserDocuments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_UserDocuments_IdentificationCardId",
                table: "ApplicationUsers",
                column: "IdentificationCardId",
                principalTable: "UserDocuments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarDocuments_Images_BackSideImageId",
                table: "CarDocuments",
                column: "BackSideImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarDocuments_Images_FrontSideImageId",
                table: "CarDocuments",
                column: "FrontSideImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Images_ProfilePicId",
                table: "Cars",
                column: "ProfilePicId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accidents_Orders_OrderId",
                table: "Accidents",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_UserDocuments_DriversLicenseId",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_UserDocuments_IdentificationCardId",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarDocuments_STKId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarDocuments_TechnicLicenseId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Images_ProfilePicId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Accidents_Orders_OrderId",
                table: "Accidents");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PDFs");

            migrationBuilder.DropTable(
                name: "StripeSubscriptions");

            migrationBuilder.DropTable(
                name: "UserDocuments");

            migrationBuilder.DropTable(
                name: "CarDocuments");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Accidents");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "CarInsurances");

            migrationBuilder.DropTable(
                name: "CarSpecifications");
        }
    }
}
