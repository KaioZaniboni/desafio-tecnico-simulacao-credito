using System.ComponentModel.DataAnnotations;

namespace SimulacaoCredito.Application.DTOs;

public class AuthenticationRequestDto
{
    [Required(ErrorMessage = "Username é obrigatório")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password é obrigatório")]
    public string Password { get; set; } = string.Empty;
}

public class AuthenticationResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public string Username { get; set; } = string.Empty;
}
