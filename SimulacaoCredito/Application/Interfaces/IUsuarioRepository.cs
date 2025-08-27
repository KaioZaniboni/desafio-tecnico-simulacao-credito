using SimulacaoCredito.Domain.Entities;

namespace SimulacaoCredito.Application.Interfaces;

public interface IUsuarioRepository
{
    /// <summary>
    /// Busca usuário por username
    /// </summary>
    /// <param name="username">Nome de usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<Usuario?> ObterPorUsernameAsync(string username);

    /// <summary>
    /// Busca usuário por ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<Usuario?> ObterPorIdAsync(long id);

    /// <summary>
    /// Busca usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<Usuario?> ObterPorEmailAsync(string email);

    /// <summary>
    /// Lista todos os usuários ativos
    /// </summary>
    /// <returns>Lista de usuários ativos</returns>
    Task<List<Usuario>> ListarUsuariosAtivosAsync();

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="usuario">Dados do usuário</param>
    /// <returns>Usuário criado com ID</returns>
    Task<Usuario> CriarAsync(Usuario usuario);

    /// <summary>
    /// Atualiza dados do usuário
    /// </summary>
    /// <param name="usuario">Usuário com dados atualizados</param>
    /// <returns>Usuário atualizado</returns>
    Task<Usuario> AtualizarAsync(Usuario usuario);

    /// <summary>
    /// Verifica se username já existe
    /// </summary>
    /// <param name="username">Nome de usuário</param>
    /// <param name="excludeId">ID para excluir da verificação (para updates)</param>
    /// <returns>True se username já existe</returns>
    Task<bool> UsernameExisteAsync(string username, long? excludeId = null);

    /// <summary>
    /// Verifica se email já existe
    /// </summary>
    /// <param name="email">Email</param>
    /// <param name="excludeId">ID para excluir da verificação (para updates)</param>
    /// <returns>True se email já existe</returns>
    Task<bool> EmailExisteAsync(string email, long? excludeId = null);

    /// <summary>
    /// Registra tentativa de login (sucesso ou falha)
    /// </summary>
    /// <param name="usuario">Usuário</param>
    /// <param name="sucesso">Se o login foi bem-sucedido</param>
    /// <returns>Task</returns>
    Task RegistrarTentativaLoginAsync(Usuario usuario, bool sucesso);
}
