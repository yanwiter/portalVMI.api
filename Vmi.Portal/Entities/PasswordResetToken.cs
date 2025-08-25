using System;
using System.ComponentModel.DataAnnotations;

namespace Vmi.Portal.Entities;

public class PasswordResetToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; }
    public Guid IdUsuario { get; set; }
    public Usuario Usuario { get; set; }
    public DateTime DataExpiracao { get; set; }
    public bool IsTokenUsado { get; set; }
    public DateTime? DataUsoToken { get; set; }
}