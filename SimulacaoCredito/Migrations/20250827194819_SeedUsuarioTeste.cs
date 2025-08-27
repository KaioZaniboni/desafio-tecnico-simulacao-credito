using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulacaoCredito.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsuarioTeste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Inserir usuário de teste com senha hasheada via BCrypt
            // Senha: "admin123" -> Hash BCrypt com work factor 12
            migrationBuilder.Sql(@"
                INSERT INTO Usuarios (Username, PasswordHash, Email, NomeCompleto, Ativo, DataCriacao, TentativasLogin, ContaBloqueada)
                VALUES (
                    'testuser',
                    '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBdXzgVmeRD1VG',
                    'testuser@simulacaocredito.com',
                    'Usuário de Teste',
                    1,
                    SYSDATETIMEOFFSET(),
                    0,
                    0
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover usuário de teste
            migrationBuilder.Sql("DELETE FROM Usuarios WHERE Username = 'testuser';");
        }
    }
}
