using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLayer.Migrations;

public partial class applicationuser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ApplicationUsers",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                IdentityUserId = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_ApplicationUsers_AspNetUsers_IdentityUserId",
                    column: x => x.IdentityUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id");
            });

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

        migrationBuilder.CreateIndex(
            name: "IX_ApplicationUsers_IdentityUserId",
            table: "ApplicationUsers",
            column: "IdentityUserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ApplicationUsers");

        migrationBuilder.UpdateData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "0b5141f7-3aed-4cf9-a51d-4ad671703e1f",
            column: "ConcurrencyStamp",
            value: "49bc9d5e-a845-4b58-a56d-8d6e882b18c4");

        migrationBuilder.UpdateData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
            column: "ConcurrencyStamp",
            value: "477098b8-998a-430f-a665-65f994672bba");

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
            columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
            values: new object[] { "3d277a58-f8b9-4eee-ab0b-72a3516a90b3", "AQAAAAEAACcQAAAAEAt8SfKYBDUcN4amfN/1EePBRhMzIfWVlqZAl84A2IVwAdLKF2lASlf+zYJYnWQhkA==", "85909a14-a3f5-49ba-bcfd-9e868a03b868" });
    }
}
