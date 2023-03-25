using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class applicationEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "ApplicationUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DriversLicenseId",
                table: "ApplicationUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentificationCardId",
                table: "ApplicationUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Accidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accidents", x => x.Id);
                });

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
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RelativePath = table.Column<string>(type: "text", nullable: false),
                    ImageType = table.Column<string>(type: "text", nullable: false),
                    CarImageType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "AccidentImages",
                columns: table => new
                {
                    AccidentsId = table.Column<int>(type: "integer", nullable: false),
                    PhotoDocumantationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccidentImages", x => new { x.AccidentsId, x.PhotoDocumantationId });
                    table.ForeignKey(
                        name: "FK_AccidentImages_Accidents_AccidentsId",
                        column: x => x.AccidentsId,
                        principalTable: "Accidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccidentImages_Images_PhotoDocumantationId",
                        column: x => x.PhotoDocumantationId,
                        principalTable: "Images",
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
                    DocNr = table.Column<string>(type: "text", nullable: false),
                    ValidTill = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarDocuments_Images_BackSideImageId",
                        column: x => x.BackSideImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarDocuments_Images_FrontSideImageId",
                        column: x => x.FrontSideImageId,
                        principalTable: "Images",
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
                    FrontSideImageId = table.Column<int>(type: "integer", nullable: false),
                    BackSideImageId = table.Column<int>(type: "integer", nullable: false),
                    Checked = table.Column<bool>(type: "boolean", nullable: false),
                    DocNr = table.Column<string>(type: "text", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDocuments_Images_FrontSideImageId",
                        column: x => x.FrontSideImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentCardId = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CanceledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentCards_PaymentCardId",
                        column: x => x.PaymentCardId,
                        principalTable: "PaymentCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    BasicRentalPrice = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Cars_Images_ProfilePicId",
                        column: x => x.ProfilePicId,
                        principalTable: "Images",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    CarId = table.Column<int>(type: "integer", nullable: false),
                    PaymentId = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccidentId = table.Column<int>(type: "integer", nullable: true),
                    Distance = table.Column<decimal>(type: "numeric", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Orders_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
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
                name: "OrderImages",
                columns: table => new
                {
                    OrdersId = table.Column<int>(type: "integer", nullable: false),
                    ReturningPhotosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderImages", x => new { x.OrdersId, x.ReturningPhotosId });
                    table.ForeignKey(
                        name: "FK_OrderImages_Images_ReturningPhotosId",
                        column: x => x.ReturningPhotosId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderImages_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b5141f7-3aed-4cf9-a51d-4ad671703e1f",
                column: "ConcurrencyStamp",
                value: "18cb4bc6-888f-4e0c-8dcf-4808ae4bd9d0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                column: "ConcurrencyStamp",
                value: "874a196f-b437-4fbc-a46f-15a47c127478");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0ff2782f-6342-46a1-8fc3-a6ec465e33fe", "AQAAAAEAACcQAAAAEC9jeXUs3DbelU4vs1sAoM5IpljEZfcIyC+wWyGfZQf5GvRu3x9r8MkOK5LK9yBGFg==", "aa99f529-4363-4be2-ba0d-49e568efd061" });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_DriversLicenseId",
                table: "ApplicationUsers",
                column: "DriversLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_IdentificationCardId",
                table: "ApplicationUsers",
                column: "IdentificationCardId");

            migrationBuilder.CreateIndex(
                name: "IX_AccidentImages_PhotoDocumantationId",
                table: "AccidentImages",
                column: "PhotoDocumantationId");

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
                name: "IX_OrderImages_ReturningPhotosId",
                table: "OrderImages",
                column: "ReturningPhotosId");

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
                name: "IX_Orders_PaymentId",
                table: "Orders",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentCardId",
                table: "Payments",
                column: "PaymentCardId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_UserDocuments_DriversLicenseId",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_UserDocuments_IdentificationCardId",
                table: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "AccidentImages");

            migrationBuilder.DropTable(
                name: "OrderImages");

            migrationBuilder.DropTable(
                name: "PDFs");

            migrationBuilder.DropTable(
                name: "UserDocuments");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Accidents");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "CarDocuments");

            migrationBuilder.DropTable(
                name: "CarInsurances");

            migrationBuilder.DropTable(
                name: "CarSpecifications");

            migrationBuilder.DropTable(
                name: "PaymentCards");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_DriversLicenseId",
                table: "ApplicationUsers");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_IdentificationCardId",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "DriversLicenseId",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "IdentificationCardId",
                table: "ApplicationUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b5141f7-3aed-4cf9-a51d-4ad671703e1f",
                column: "ConcurrencyStamp",
                value: "2598b73f-5d6d-472e-b487-53ee198fe26a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                column: "ConcurrencyStamp",
                value: "d63b2334-d573-485e-ba35-dd62ebedb92a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c1b81abd-c0f6-46f9-9f12-811be8085cb5", "AQAAAAEAACcQAAAAEPZeILv4pvK9+JNA1kpgUXHWVzQ7/WwQ4+2psOYayA2r81ZThmSF25dX3drDsC2tNA==", "7d379e50-2e88-410c-8695-63706ee01a62" });
        }
    }
}
