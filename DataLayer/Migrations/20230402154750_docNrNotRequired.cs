using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class docNrNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocNr",
                table: "UserDocuments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

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
                value: "591df0cb-6882-4431-bd29-2ba1019934e3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                column: "ConcurrencyStamp",
                value: "b17fb4a3-43ff-4340-a39c-33b26fe07e6e");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "06910eb9-5f6f-41e9-9d29-200eddde23c9", "AQAAAAEAACcQAAAAEBRMxh7Sf7uJ5H9rgquZg+oH7BDkxYsBjvFsFsMWqplOJ9M3Pe985NFBYyG5mbl9wg==", "70c956d7-24e8-4006-b1bc-c29246117140" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocNr",
                table: "UserDocuments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
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
        }
    }
}
