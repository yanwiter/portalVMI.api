using System;
using System.Threading.Tasks;
using Vmi.Portal.Data.Repositories.Interfaces;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Services.Interfaces;

namespace Vmi.Portal.Services
{
    public class AuthenticationCodeService : IAuthenticationCodeService
    {
        private readonly IAuthenticationCodeRepository _authenticationCodeRepository;
        private readonly IEmailService _emailService;
        private readonly IUsuarioService _usuarioService;

        public AuthenticationCodeService(
            IAuthenticationCodeRepository authCodeRepository,
            IEmailService emailService,
            IUsuarioService usuarioService)
        {
            _authenticationCodeRepository = authCodeRepository;
            _emailService = emailService;
            _usuarioService = usuarioService;
        }

        public async Task SaveAuthCode(Guid userId, string code, DateTime expiration)
        {
            await _authenticationCodeRepository.SaveAuthCodeAsync(userId, code, expiration);
        }

        public async Task<bool> ValidateCode(Guid userId, string code)
        {
            return await _authenticationCodeRepository.ValidateCodeAsync(userId, code);
        }

        public async Task ResendCode(Guid userId)
        {
            var user = await _usuarioService.ObterUsuarioPorId(userId);
            if (user == null) return;

            var newCode = new Random().Next(100000, 999999).ToString();
            var expiration = DateTime.UtcNow.AddMinutes(10);

            await SaveAuthCode(userId, newCode, expiration);
            await SendAuthenticationCode(user.Email, user.Nome, newCode);
        }

        public async Task SendAuthenticationCode(string email, string name, string code)
        {
            var subject = "Seu código de autenticação";
            var body = $@"
                <!DOCTYPE html>
                <html lang='pt-BR'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Código de Autenticação</title>
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
                            background: linear-gradient(180deg, #1a2530 100%, #2c3e50 0%);
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

                        .code-container {{
                            text-align: center;
                            margin: 30px 0;
                        }}

                        .code {{
                            display: inline-block;
                            background: #f5f7fa;
                            color: #1a2530;
                            font-size: 28px;
                            font-weight: 600;
                            padding: 15px 30px;
                            border-radius: 6px;
                            letter-spacing: 3px;
                            border: 1px dashed #1a2530;
                        }}

                        .footer {{
                            text-align: center;
                            padding: 20px;
                            background: linear-gradient(180deg, #1a2530 100%, #2c3e50 0%);
                            font-size: 12px;
                            color: #777;
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

                        .info {{
                            background-color: #f5f7fa;
                            padding: 15px;
                            border-radius: 6px;
                            margin: 20px 0;
                            text-align: center;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <img src='https://vmimedica.com/wp-content/uploads/2021/07/vmi-medica-logo.svg' alt='Portal PGA' class='logo'>
                            <h1>Código de Autenticação</h1>
                        </div>

                        <div class='content'>
                            <p>Olá, {name}!</p>

                            <p>Abaixo está o código de autenticação para acessar o Portal PGA.</p>
                
                            <div class='info'>
                                <p>Por favor, utilize o seguinte código para completar seu acesso ao portal:</p>
                                <div class='code-container'>
                                    <div class='code'>{code}</div>
                                </div>
                                <p>Este código é válido por <strong>10 minutos</strong>.</p>
                            </div>

                            <div class='divider'></div>

                            <p>Se você não solicitou este código, por favor ignore este e-mail ou entre em contato com nosso suporte imediatamente.</p>
                        </div>

                        <div class='footer'>
                            <p>© {DateTime.Now.Year} Portal PGA. Todos os direitos reservados.</p>
                            <p>Este é um email automático, por favor não responda.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(email, subject, body, true);
        }

        public async Task CleanExpiredCodes()
        {
            await _authenticationCodeRepository.CleanExpiredCodesAsync();
        }
    }
}