using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Proposta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResetPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "PropostaStatus",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostaStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propostas",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteNome = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propostas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propostas_PropostaStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "public",
                        principalTable: "PropostaStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "PropostaStatus",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "EmAnalise" },
                    { 2, "Aprovada" },
                    { 3, "Rejeitada" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Propostas",
                columns: new[] { "Id", "ClienteNome", "CriadaEm", "StatusId", "Valor" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "João Silva", new DateTime(2026, 5, 2, 0, 17, 30, 285, DateTimeKind.Utc).AddTicks(1905), 1, 1200m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Maria Oliveira", new DateTime(2026, 5, 2, 0, 17, 30, 285, DateTimeKind.Utc).AddTicks(1911), 2, 2300m },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Ana Luiza", new DateTime(2026, 5, 2, 0, 17, 30, 285, DateTimeKind.Utc).AddTicks(1914), 3, 2300m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_StatusId",
                schema: "public",
                table: "Propostas",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Propostas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PropostaStatus",
                schema: "public");
        }
    }
}
