using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Infrastructure.Security;

namespace SimulacaoCredito.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtService jwtService,
        IUsuarioRepository usuarioRepository,
        IPasswordHashService passwordHashService,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _usuarioRepository = usuarioRepository;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    /// <summary>
    /// Gera um token JWT para autenticação
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT válido</returns>
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponseDto>> GenerateToken([FromBody] AuthenticationRequestDto request)
    {
        _logger.LogInformation("Tentativa de autenticação para usuário: {Username}", request.Username);

        try
        {
            // Buscar usuário no banco de dados
            var usuario = await _usuarioRepository.ObterPorUsernameAsync(request.Username);

            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado: {Username}", request.Username);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Credenciais inválidas",
                    Detail = "Username ou password incorretos",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Verificar se a conta pode autenticar
            if (!usuario.PodeAutenticar)
            {
                _logger.LogWarning("Conta não pode autenticar - Ativo: {Ativo}, Bloqueada: {Bloqueada}, Username: {Username}",
                    usuario.Ativo, usuario.ContaBloqueada, request.Username);

                var detail = usuario.ContaBloqueada
                    ? "Conta bloqueada devido a múltiplas tentativas de login falhadas"
                    : "Conta inativa";

                return Unauthorized(new ProblemDetails
                {
                    Title = "Acesso negado",
                    Detail = detail,
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Verificar senha
            var senhaValida = _passwordHashService.VerifyPassword(request.Password, usuario.PasswordHash);

            if (!senhaValida)
            {
                _logger.LogWarning("Senha inválida para usuário: {Username}", request.Username);

                // Registrar tentativa de login falhada
                await _usuarioRepository.RegistrarTentativaLoginAsync(usuario, false);

                return Unauthorized(new ProblemDetails
                {
                    Title = "Credenciais inválidas",
                    Detail = "Username ou password incorretos",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Login bem-sucedido
            await _usuarioRepository.RegistrarTentativaLoginAsync(usuario, true);

            var token = _jwtService.GenerateToken(request.Username);
            var expiresAt = DateTime.UtcNow.AddMinutes(60); // Configurável via appsettings

            _logger.LogInformation("Token gerado com sucesso para usuário: {Username}", request.Username);

            return Ok(new AuthenticationResponseDto
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresAt = expiresAt,
                Username = request.Username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante autenticação do usuário: {Username}", request.Username);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Erro interno",
                Detail = "Erro interno do servidor durante autenticação",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }



    /// <summary>
    /// Endpoint para validar se o token atual ainda é válido
    /// </summary>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult ValidateToken()
    {
        var username = User.Identity?.Name;
        _logger.LogInformation("Token validado com sucesso para usuário: {Username}", username);
        
        return Ok(new { 
            Valid = true, 
            Username = username,
            ValidatedAt = DateTime.UtcNow 
        });
    }
}
