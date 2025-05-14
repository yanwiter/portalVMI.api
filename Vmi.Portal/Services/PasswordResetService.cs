using Vmi.Portal.Data.Repositories.Interfaces;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories;
using Vmi.Portal.Services.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace Vmi.Portal.Services;
public class PasswordResetService : IPasswordResetService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public PasswordResetService(
        IUsuarioRepository usuarioRepository,
        IPasswordResetTokenRepository tokenRepository,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Message)> GerarTokenRedefinicaoSenha(string email)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmail(email);
        if (usuario == null)
        {
            return (true, "Se o e-mail estiver cadastrado, você receberá um link para redefinir sua senha.");
        }

        var token = Guid.NewGuid().ToString();
        var dataExpiracao = DateTime.UtcNow.AddHours(24);

        var resetToken = new PasswordResetToken
        {
            Token = token,
            UsuarioId = usuario.Id,
            DataExpiracao = dataExpiracao,
            IsTokenUsado = false
        };

        await _tokenRepository.CriarTokenRedefinicaoSenhaAsync(resetToken);

        var resetLink = $"{_configuration["Frontend:Url"]}/redefinir-senha?token={token}";

        var emailBody = GerarEmailResetSenha(resetLink, usuario.Nome);

        await _emailService.SendEmailAsync(
            email,
            "Redefinição de Senha - VMI Médica",
            emailBody,
            isHtml: true);

        return (true, "Se o e-mail estiver cadastrado, você receberá um link para redefinir sua senha.");
    }

    private string GerarEmailResetSenha(string resetLink, string userName)
    {
        return $@"
            <!DOCTYPE html>
            <html lang='pt-BR'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Redefinição de Senha</title>
                <style>
                    @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600&display=swap');
        
                    body {{
                        font-family: 'Poppins', Arial, sans-serif;
                        background-color: #f5f7fa;
                        margin: 0;
                        padding: 0;
                        color: #333;
                        line-height: 1.6;
                    }}
        
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background: #ffffff;
                        border-radius: 10px;
                        overflow: hidden;
                        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
                    }}
        
                    .header {{
                        background: #b38a5e;
                        padding: 30px 20px;
                        text-align: center;
                        color: white;
                    }}
        
                    .header h1 {{
                        margin: 0;
                        font-size: 24px;
                        font-weight: 600;
                    }}
        
                    .content {{
                        padding: 30px;
                    }}
        
                    .content p {{
                        margin-bottom: 20px;
                    }}
        
                    .button-container {{
                        text-align: center;
                        margin: 30px 0;
                    }}
        
                    .button {{
                        display: inline-block;
                        background: #b38a5e;
                        color: white !important;
                        text-decoration: none;
                        padding: 12px 30px;
                        border-radius: 6px;
                        font-weight: 500;
                        font-size: 16px;
                        transition: all 0.3s ease;
                        box-shadow: 0 4px 10px rgba(110, 142, 251, 0.3);
                    }}
        
                    .button:hover {{
                        transform: translateY(-2px);
                        box-shadow: 0 6px 15px rgba(110, 142, 251, 0.4);
                    }}
        
                    .footer {{
                        text-align: center;
                        padding: 20px;
                        background-color: #f9f9f9;
                        font-size: 12px;
                        color: #777;
                    }}
        
                    .link {{
                        word-break: break-all;
                        color: #6e8efb;
                        text-decoration: none;
                    }}
        
                    .logo {{
                        max-width: 150px;
                        margin-bottom: 15px;
                    }}
        
                    .divider {{
                        height: 1px;
                        background-color: #eee;
                        margin: 25px 0;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <img src='https://vmimedica.com/wp-content/uploads/2021/07/vmi-medica-logo.svg' alt='VMI Médica' class='logo'>
                        <h1>Redefinição de Senha</h1>
                    </div>
        
                    <div class='content'>
                        <p>Olá, {userName}!</p>
            
                        <p>Recebemos uma solicitação para redefinir a senha da sua conta. Clique no botão abaixo para criar uma nova senha:</p>
            
                        <div class='button-container'>
                            <a href='{resetLink}' class='button'>Redefinir Senha</a>
                        </div>
            
                        <p>Se você não solicitou uma redefinição de senha, por favor ignore este email ou entre em contato conosco se tiver dúvidas.</p>
            
                        <div class='divider'></div>
            
                        <p>Caso o botão acima não funcione, copie e cole o link abaixo no seu navegador:</p>
                        <p><a href='{resetLink}' class='link'>{resetLink}</a></p>
                    </div>
        
                    <div class='footer'>
                        <p>© {DateTime.Now.Year} VMI Médica. Todos os direitos reservados.</p>
                        <p>Este é um email automático, por favor não responda.</p>
                    </div>
                </div>
            </body>
            </html>";
    }

    public async Task<bool> ValidarTokenRedefinicaoSenha(string token)
    {
        var resetToken = await _tokenRepository.PegarTokenValidoAsync(token);
        return resetToken != null;
    }

    public async Task<(bool Success, string Message)> RedefinirSenha(string token, string novaSenha)
    {
        var resetToken = await _tokenRepository.PegarTokenValidoAsync(token);
        if (resetToken == null)
        {
            return (false, "Token inválido ou expirado.");
        }

        resetToken.Usuario.Senha = BC.HashPassword(novaSenha);

        await _usuarioRepository.AtualizarUsuario(resetToken.Usuario);

        resetToken.IsTokenUsado = true;
        resetToken.DataUsoToken = DateTime.UtcNow;
        await _tokenRepository.AtualizarTokenRedefinicaoSenhaAsync(resetToken);

        return (true, "Senha redefinida com sucesso.");
    }
}