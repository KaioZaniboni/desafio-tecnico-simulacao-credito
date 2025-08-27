using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Domain.Entities;
using SimulacaoCredito.Infrastructure.Data;

namespace SimulacaoCredito.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UsuarioRepository> _logger;

    public UsuarioRepository(AppDbContext context, ILogger<UsuarioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Usuario?> ObterPorUsernameAsync(string username)
    {
        _logger.LogDebug("Buscando usuário por username: {Username}", username);
        
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<Usuario?> ObterPorIdAsync(long id)
    {
        _logger.LogDebug("Buscando usuário por ID: {Id}", id);
        
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        _logger.LogDebug("Buscando usuário por email: {Email}", email);
        
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
    }

    public async Task<List<Usuario>> ListarUsuariosAtivosAsync()
    {
        _logger.LogDebug("Listando usuários ativos");
        
        return await _context.Usuarios
            .Where(u => u.Ativo)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<Usuario> CriarAsync(Usuario usuario)
    {
        _logger.LogInformation("Criando novo usuário: {Username}", usuario.Username);
        
        usuario.DataCriacao = DateTimeOffset.UtcNow;
        
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Usuário criado com sucesso. ID: {Id}, Username: {Username}", 
            usuario.Id, usuario.Username);
        
        return usuario;
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        _logger.LogInformation("Atualizando usuário: {Id} - {Username}", usuario.Id, usuario.Username);
        
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Usuário atualizado com sucesso: {Id} - {Username}", 
            usuario.Id, usuario.Username);
        
        return usuario;
    }

    public async Task<bool> UsernameExisteAsync(string username, long? excludeId = null)
    {
        var query = _context.Usuarios.Where(u => u.Username.ToLower() == username.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> EmailExisteAsync(string email, long? excludeId = null)
    {
        var query = _context.Usuarios.Where(u => u.Email != null && u.Email.ToLower() == email.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task RegistrarTentativaLoginAsync(Usuario usuario, bool sucesso)
    {
        _logger.LogInformation("Registrando tentativa de login para usuário {Username}: {Resultado}", 
            usuario.Username, sucesso ? "Sucesso" : "Falha");
        
        if (sucesso)
        {
            usuario.RegistrarLoginSucesso();
        }
        else
        {
            usuario.RegistrarLoginFalha();
        }
        
        await AtualizarAsync(usuario);
        
        if (!sucesso && usuario.ContaBloqueada)
        {
            _logger.LogWarning("Conta bloqueada após múltiplas tentativas falhadas: {Username}", 
                usuario.Username);
        }
    }
}
