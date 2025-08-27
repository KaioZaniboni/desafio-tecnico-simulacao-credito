using BCrypt.Net;

namespace SimulacaoCredito.Infrastructure.Security;

public interface IPasswordHashService
{
    /// <summary>
    /// Gera hash da senha usando BCrypt
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <returns>Hash da senha</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica se a senha corresponde ao hash
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <param name="hash">Hash armazenado</param>
    /// <returns>True se a senha está correta</returns>
    bool VerifyPassword(string password, string hash);
}

public class PasswordHashService : IPasswordHashService
{
    private readonly ILogger<PasswordHashService> _logger;

    public PasswordHashService(ILogger<PasswordHashService> logger)
    {
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));
        }

        _logger.LogDebug("Gerando hash para senha");

        // Usar work factor 12 para boa segurança vs performance
        var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        
        _logger.LogDebug("Hash gerado com sucesso");
        
        return hash;
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogDebug("Senha vazia fornecida para verificação");
            return false;
        }

        if (string.IsNullOrWhiteSpace(hash))
        {
            _logger.LogDebug("Hash vazio fornecido para verificação");
            return false;
        }

        try
        {
            _logger.LogDebug("Verificando senha contra hash");
            
            var isValid = BCrypt.Net.BCrypt.Verify(password, hash);
            
            _logger.LogDebug("Verificação de senha: {Resultado}", isValid ? "Válida" : "Inválida");
            
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar senha contra hash");
            return false;
        }
    }
}
