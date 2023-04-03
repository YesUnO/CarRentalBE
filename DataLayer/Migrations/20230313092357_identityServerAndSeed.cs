using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations;

public partial class identityServerAndSeed : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AspNetUserTokens",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(128)",
            oldMaxLength: 128);

        migrationBuilder.AlterColumn<string>(
            name: "LoginProvider",
            table: "AspNetUserTokens",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(128)",
            oldMaxLength: 128);

        migrationBuilder.AlterColumn<string>(
            name: "ProviderKey",
            table: "AspNetUserLogins",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(128)",
            oldMaxLength: 128);

        migrationBuilder.AlterColumn<string>(
            name: "LoginProvider",
            table: "AspNetUserLogins",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(128)",
            oldMaxLength: 128);

        migrationBuilder.CreateTable(
            name: "DeviceCodes",
            columns: table => new
            {
                UserCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                DeviceCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                SubjectId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                SessionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ClientId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Data = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeviceCodes", x => x.UserCode);
            });

        migrationBuilder.CreateTable(
            name: "Keys",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false),
                Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Use = table.Column<string>(type: "text", nullable: true),
                Algorithm = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                IsX509Certificate = table.Column<bool>(type: "boolean", nullable: false),
                DataProtected = table.Column<bool>(type: "boolean", nullable: false),
                Data = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Keys", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PersistedGrants",
            columns: table => new
            {
                Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                SubjectId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                SessionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ClientId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ConsumedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Data = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PersistedGrants", x => x.Key);
            });

        migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            values: new object[,]
            {
                { "0b5141f7-3aed-4cf9-a51d-4ad671703e1f", "49bc9d5e-a845-4b58-a56d-8d6e882b18c4", "Customer", "CUSTOMER" },
                { "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7", "477098b8-998a-430f-a665-65f994672bba", "Admin", "ADMIN" }
            });

        migrationBuilder.InsertData(
            table: "AspNetUsers",
            columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
            values: new object[] { "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406", 0, "3d277a58-f8b9-4eee-ab0b-72a3516a90b3", "vilem.cech@gmail.com", true, false, null, "VILEM.CECH@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEAt8SfKYBDUcN4amfN/1EePBRhMzIfWVlqZAl84A2IVwAdLKF2lASlf+zYJYnWQhkA==", "773951604", true, "85909a14-a3f5-49ba-bcfd-9e868a03b868", false, "admin" });

        migrationBuilder.InsertData(
            table: "AspNetUserRoles",
            columns: new[] { "RoleId", "UserId" },
            values: new object[] { "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7", "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406" });

        migrationBuilder.CreateIndex(
            name: "IX_DeviceCodes_DeviceCode",
            table: "DeviceCodes",
            column: "DeviceCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_DeviceCodes_Expiration",
            table: "DeviceCodes",
            column: "Expiration");

        migrationBuilder.CreateIndex(
            name: "IX_Keys_Use",
            table: "Keys",
            column: "Use");

        migrationBuilder.CreateIndex(
            name: "IX_PersistedGrants_ConsumedTime",
            table: "PersistedGrants",
            column: "ConsumedTime");

        migrationBuilder.CreateIndex(
            name: "IX_PersistedGrants_Expiration",
            table: "PersistedGrants",
            column: "Expiration");

        migrationBuilder.CreateIndex(
            name: "IX_PersistedGrants_SubjectId_ClientId_Type",
            table: "PersistedGrants",
            columns: new[] { "SubjectId", "ClientId", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_PersistedGrants_SubjectId_SessionId_Type",
            table: "PersistedGrants",
            columns: new[] { "SubjectId", "SessionId", "Type" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DeviceCodes");

        migrationBuilder.DropTable(
            name: "Keys");

        migrationBuilder.DropTable(
            name: "PersistedGrants");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "0b5141f7-3aed-4cf9-a51d-4ad671703e1f");

        migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "RoleId", "UserId" },
            keyValues: new object[] { "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7", "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406" });

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7");

        migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AspNetUserTokens",
            type: "character varying(128)",
            maxLength: 128,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AlterColumn<string>(
            name: "LoginProvider",
            table: "AspNetUserTokens",
            type: "character varying(128)",
            maxLength: 128,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AlterColumn<string>(
            name: "ProviderKey",
            table: "AspNetUserLogins",
            type: "character varying(128)",
            maxLength: 128,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AlterColumn<string>(
            name: "LoginProvider",
            table: "AspNetUserLogins",
            type: "character varying(128)",
            maxLength: 128,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");
    }
}
