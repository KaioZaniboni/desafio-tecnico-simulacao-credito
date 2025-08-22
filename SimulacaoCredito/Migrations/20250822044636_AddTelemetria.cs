using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulacaoCredito.Migrations
{
    /// <inheritdoc />
    public partial class AddTelemetria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelemetriaRequisicao",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeApi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TempoResposta = table.Column<int>(type: "int", nullable: false),
                    Sucesso = table.Column<bool>(type: "bit", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    Erro = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UsuarioId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnderecoIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetriaRequisicao", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetriaRequisicao_DataHora",
                schema: "dbo",
                table: "TelemetriaRequisicao",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetriaRequisicao_NomeApi",
                schema: "dbo",
                table: "TelemetriaRequisicao",
                column: "NomeApi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemetriaRequisicao",
                schema: "dbo");
        }
    }
}
