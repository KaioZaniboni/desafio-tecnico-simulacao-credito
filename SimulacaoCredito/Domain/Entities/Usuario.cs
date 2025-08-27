using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulacaoCredito.Domain.Entities;

[Table("Usuarios")]
public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    [Column("Username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(100)]
    [Column("Email")]
    public string? Email { get; set; }

    [StringLength(100)]
    [Column("NomeCompleto")]
    public string? NomeCompleto { get; set; }

    [Column("Ativo")]
    public bool Ativo { get; set; } = true;

    [Column("DataCriacao")]
    public DateTimeOffset DataCriacao { get; set; } = DateTimeOffset.UtcNow;

    [Column("DataUltimoLogin")]
    public DateTimeOffset? DataUltimoLogin { get; set; }

    [Column("TentativasLogin")]
    public int TentativasLogin { get; set; } = 0;

    [Column("ContaBloqueada")]
    public bool ContaBloqueada { get; set; } = false;

    [Column("DataBloqueio")]
    public DateTimeOffset? DataBloqueio { get; set; }

    /// <summary>
    /// Verifica se a conta está ativa e não bloqueada
    /// </summary>
    public bool PodeAutenticar => Ativo && !ContaBloqueada;

    /// <summary>
    /// Registra tentativa de login bem-sucedida
    /// </summary>
    public void RegistrarLoginSucesso()
    {
        DataUltimoLogin = DateTimeOffset.UtcNow;
        TentativasLogin = 0;
        ContaBloqueada = false;
        DataBloqueio = null;
    }

    /// <summary>
    /// Registra tentativa de login falhada
    /// </summary>
    public void RegistrarLoginFalha()
    {
        TentativasLogin++;
        
        // Bloquear conta após 5 tentativas falhadas
        if (TentativasLogin >= 5)
        {
            ContaBloqueada = true;
            DataBloqueio = DateTimeOffset.UtcNow;
        }
    }

    /// <summary>
    /// Desbloqueia a conta manualmente
    /// </summary>
    public void DesbloquearConta()
    {
        ContaBloqueada = false;
        DataBloqueio = null;
        TentativasLogin = 0;
    }
}
