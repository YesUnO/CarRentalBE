using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class documentFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_Images_BackSideImageId",
                table: "UserDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_Images_FrontSideImageId",
                table: "UserDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "FrontSideImageId",
                table: "UserDocuments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "DocNr",
                table: "UserDocuments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "BackSideImageId",
                table: "UserDocuments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "DocNr",
                table: "CarDocuments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

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
                name: "FK_UserDocuments_Images_BackSideImageId",
                table: "UserDocuments",
                column: "BackSideImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocuments_Images_FrontSideImageId",
                table: "UserDocuments",
                column: "FrontSideImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_Images_BackSideImageId",
                table: "UserDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_Images_FrontSideImageId",
                table: "UserDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "FrontSideImageId",
                table: "UserDocuments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocNr",
                table: "UserDocuments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BackSideImageId",
                table: "UserDocuments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocNr",
                table: "CarDocuments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b5141f7-3aed-4cf9-a51d-4ad671703e1f",
                column: "ConcurrencyStamp",
                value: "07a71494-7719-4eec-9d0b-3ad3de76a559");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                column: "ConcurrencyStamp",
                value: "34fc709c-c7f8-4cc5-a3ae-2e4c0d8598d1");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d4d00e78-b183-4483-a6a5-b48970a96e09", "AQAAAAEAACcQAAAAEOZKMJrncqSgkuiUe/toCZg9H4hPtrbSlZgQfEy0VClcsCpqj4YykRFzDNqv149WNg==", "4a7c59ed-0048-4956-81d5-f80ca66ba8f0" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocuments_Images_BackSideImageId",
                table: "UserDocuments",
                column: "BackSideImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocuments_Images_FrontSideImageId",
                table: "UserDocuments",
                column: "FrontSideImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
