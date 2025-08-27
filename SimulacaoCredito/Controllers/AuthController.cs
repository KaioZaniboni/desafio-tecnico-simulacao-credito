using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Infrastructure.Security;

namespace SimulacaoCredito.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IJwtService jwtService, ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
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
    public ActionResult<AuthenticationResponseDto> GenerateToken([FromBody] AuthenticationRequestDto request)
    {
        _logger.LogInformation("Tentativa de autenticação para usuário: {Username}", request.Username);

        // Validação básica das credenciais (em produção, validar contra base de dados)
        if (!IsValidCredentials(request.Username, request.Password))
        {
            _logger.LogWarning("Credenciais inválidas para usuário: {Username}", request.Username);
            return Unauthorized(new ProblemDetails
            {
                Title = "Credenciais inválidas",
                Detail = "Username ou password incorretos",
                Status = StatusCodes.Status401Unauthorized
            });
        }

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

    /// <summary>
    /// Valida se as credenciais são válidas
    /// Em produção, isso deveria consultar uma base de dados ou serviço de identidade
    /// </summary>
    private static bool IsValidCredentials(string username, string password)
    {
        // Credenciais hardcoded para demonstração
        // Em produção, validar contra base de dados com hash da senha
        var validCredentials = new Dictionary<string, string>
        {
            { "admin", "123456" },
            { "user", "password" },
            { "test", "test123" }
        };

        return validCredentials.TryGetValue(username, out var validPassword) && 
               validPassword == password;
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
