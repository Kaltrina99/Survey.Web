using Microsoft.EntityFrameworkCore.Migrations;

namespace Survey.Infrastructure.Migrations
{
    public partial class categoryuser1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProjectCategories",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProjectCategories", x => new { x.UserId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_UserProjectCategories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProjectCategories_ProjectCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProjectCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "2814ad01-caab-431d-8b62-a5d8e5a4090d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "3d2240e0-a676-4c8e-a6dc-28ecd056a6a6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "fe09ca95-c8a4-411b-841d-9100b00fb5e3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                column: "ConcurrencyStamp",
                value: "d1745d41-5b85-4983-9ed4-7f3432861539");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5",
                column: "ConcurrencyStamp",
                value: "fe100865-10e6-4cd4-8c5f-c65f8510b074");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8ebe4891-6252-4372-838c-fc1ba502ae12", "AQAAAAEAACcQAAAAECS1vcm+Yt8Ln++loHId8DIc8TaAK/CR5zdNS0YJKzJXvTAtM7i28wseJuw7L5Pw+Q==", "59dfa45f-94da-4156-83ba-ac186822da4a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7f286422-c0a4-43bc-9b41-3da00b85b659", "AQAAAAEAACcQAAAAEAalXHfU6q29612itlzBbiG5AXBES9epaVt0QX9keKY2ilvvmT1lukx/BYF4neWtgw==", "2806ae11-fea7-4a42-9ea2-13cc0869db5b" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectCategories_CategoryId",
                table: "UserProjectCategories",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProjectCategories");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "102df05b-0506-4824-ae64-275bdc0f9de7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "ca1efc7a-7ac3-4f7f-9add-67047404e670");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "af816b27-8ea5-4291-b83b-d5e7992d0be9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                column: "ConcurrencyStamp",
                value: "45429c5d-be63-4ec0-8991-e12ead72ce52");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5",
                column: "ConcurrencyStamp",
                value: "0b4c2f8d-d081-4e19-b3e6-3ea4597c581b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "659c693e-9be1-4683-bd84-b6a82f868b2b", "AQAAAAEAACcQAAAAEG/aiFAIV2pnJKYTtGrAy+B3ks4GFTl14m5lJqR3TuVC9KqTWrR30gClHSWeGux1ow==", "fbac9b38-86df-44ce-be3f-afa3089c0048" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c4863980-b3a1-4916-9f84-9d69699578cc", "AQAAAAEAACcQAAAAEKbSXGo82W3KBDBGUKad+WsMdesdvo4EuaW9S1rhwgdc6qoZEFnI5GyOVYq7NYIL8g==", "742814eb-c7f5-4455-b937-243b9524099c" });
        }
    }
}
