using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulacaoCredito.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Simulacao",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    ValorDesejado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Prazo = table.Column<int>(type: "int", nullable: false),
                    CodigoProduto = table.Column<int>(type: "int", nullable: false),
                    DescricaoProduto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaxaJuros = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    ValorTotalParcelas = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parcela",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimulacaoId = table.Column<long>(type: "bigint", nullable: false),
                    TipoAmortizacao = table.Column<byte>(type: "tinyint", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    ValorAmortizacao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorJuros = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorPrestacao = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcela", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcela_Simulacao_SimulacaoId",
                        column: x => x.SimulacaoId,
                        principalSchema: "dbo",
                        principalTable: "Simulacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parcela_SimulacaoId_TipoAmortizacao",
                schema: "dbo",
                table: "Parcela",
                columns: new[] { "SimulacaoId", "TipoAmortizacao" });

            migrationBuilder.CreateIndex(
                name: "IX_Simulacao_CodigoProduto",
                schema: "dbo",
                table: "Simulacao",
                column: "CodigoProduto");

            migrationBuilder.CreateIndex(
                name: "IX_Simulacao_DataCriacao",
                schema: "dbo",
                table: "Simulacao",
                column: "DataCriacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parcela",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Simulacao",
                schema: "dbo");
        }
    }
}
