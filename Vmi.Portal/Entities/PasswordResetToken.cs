using System;
using System.ComponentModel.DataAnnotations;

namespace Vmi.Portal.Entities;

public class PasswordResetToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
    public DateTime DataExpiracao { get; set; }
    public bool IsTokenUsado { get; set; }
    public DateTime? DataUsoToken { get; set; }
}