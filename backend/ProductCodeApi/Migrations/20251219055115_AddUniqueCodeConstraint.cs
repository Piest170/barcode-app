using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCodeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueCodeConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCodes_Code",
                table: "ProductCodes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCodes");
        }
    }
}
