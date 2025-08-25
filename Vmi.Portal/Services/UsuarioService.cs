using ClosedXML.Excel;
using Vmi.Portal.Common;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories;
using Vmi.Portal.Services.Interfaces;
using Vmi.Portal.Utils;
using Vmi.Portal.Enums;
using BC = BCrypt.Net.BCrypt;

namespace Vmi.Portal.Services;
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        IConfiguration configuration
        )
    {
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<PagedResult<Usuario>> ObterTodosUsuarios(int pageNumber, int pageSize, string nome, string email, Guid? idPerfil, DateTime? dataCriacao, bool? statusAcesso)
    {
        return await _usuarioRepository.ObterTodosUsuarios(pageNumber, pageSize, nome, email, idPerfil, dataCriacao, statusAcesso);
    }

    public async Task<Usuario> ObterUsuarioPorId(Guid id)
    {
        return await _usuarioRepository.ObterUsuarioPorId(id);
    }

    public async Task<Usuario> ObterUsuarioPorEmail(string email)
    {
        return await _usuarioRepository.ObterUsuarioPorEmail(email);
    }

    public async Task AtualizarFotoPerfil(Guid id, string fotoPerfil)
    {
        await _usuarioRepository.AtualizarFotoPerfil(id, fotoPerfil);
    }

    public async Task RemoverFotoPerfil(Guid id)
    {
        await _usuarioRepository.RemoverFotoPerfil(id);
    }

    public async Task AdicionarUsuario(Usuario usuario)
    {
        // Definir valores padrão para campos de suspensão
        usuario.TipoSuspensao = null;
        usuario.DataInicioSuspensao = null;
        usuario.DataFimSuspensao = null;
        usuario.MotivoSuspensao = null;
        usuario.IdRespSuspensao = null;
        usuario.NomeRespSuspensao = null;
        usuario.DataSuspensao = null;
        
        await _usuarioRepository.AdicionarUsuario(usuario);
    }

    public async Task<(bool Success, string Message)> GerarEmailBoasVindas(string email)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmail(email);
        if (usuario == null)
        {
            return (true, "Erro ao buscar usuário por E-mail");
        }

        var linkApp = $"{_configuration["Frontend:Url"]}/";

        var emailBody = EnviarEmailBoasVindas(linkApp, usuario.Nome, usuario.Email, usuario.Senha);

        await _emailService.SendEmailAsync(
            email,
            "Portal VMI - Boas-vindas",
            emailBody,
            isHtml: true);

        return (true, "E-mail de acesso inicial enviado ao usuário criado!");
    }

    private string EnviarEmailBoasVindas(string appLink, string userName, string userEmail, string tempPassword)
    {
        return $@"
        <!DOCTYPE html>
        <html lang='pt-BR'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Acesso ao Sistema</title>
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

                .button-container {{
                    text-align: center;
                    margin: 30px 0;
                }}

                .button {{
                    display: inline-block;
                    background: linear-gradient(180deg, #1a2530 100%, #2c3e50 0%);
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
                    background: linear-gradient(180deg, #1a2530 100%, #2c3e50 0%);
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

                .credentials {{
                    background-color: #f5f7fa;
                    padding: 15px;
                    border-radius: 6px;
                    margin: 20px 0;
                }}

                .credentials p {{
                    margin: 5px 0;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <img src='https://vmimedica.com/wp-content/uploads/2021/07/vmi-medica-logo.svg' alt='Portal PGA' class='logo'>
                    <h1>Bem-vindo ao Portal PGA</h1>
                </div>

                <div class='content'>
                    <p>Olá, {userName}!</p>
    
                    <p>Seu acesso ao sistema Portal PGA foi disponibilizado! Abaixo estão suas credenciais temporárias:</p>
                
                    <div class='credentials'>
                        <p><strong>Login: </strong>{userEmail}</p>
                        <p><strong>Senha temporária: </strong>{tempPassword}</p>
                    </div>
    
                    <p>Recomendamos que você altere sua senha temporária após o primeiro login.</p>
    
                    <div class='button-container'>
                        <a href='{appLink}' class='button'>Acessar o Sistema</a>
                    </div>
    
                    <div class='divider'></div>
    
                    <p>Se você não solicitou este acesso ou não reconhece esta ação, por favor entre em contato imediatamente com nosso suporte.</p>
                </div>

                <div class='footer'>
                    <p>© {DateTime.Now.Year} Portal PGA. Todos os direitos reservados.</p>
                    <p>Este é um email automático, por favor não responda.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    public async Task AtualizarUsuario(Usuario usuario)
    {
        if (usuario.StatusUsuario != StatusUsuarioEnum.Ativo && usuario.TipoSuspensao.HasValue)
        {
            if (usuario.DataSuspensao == null)
            {
                usuario.DataSuspensao = DateTime.Now;
            }
        }
        
        await _usuarioRepository.AtualizarUsuario(usuario);
    }

    public async Task RemoverUsuario(Guid id)
    {
        await _usuarioRepository.DeletarUsuario(id);
    }

    public async Task<byte[]> ExportarUsuariosParaExcel(
        int pageNumber, int pageSize,
        string nome = null,
        string email = null,
        Guid? perfilId = null,
        DateTime? dataCriacao = null,
        bool? statusAcesso = null)
    {
        var usuariosPaginados = await _usuarioRepository.ObterTodosUsuarios(pageNumber, pageSize, nome, email, perfilId, dataCriacao, statusAcesso);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Usuários");

            string[] headers = {
                "ID",
                "Nome",
                "Email",
                "Perfil",
                "Status",
                "Data de Inclusão",
                "Resp. Inclusão",
                "Data Últ. Modificação",
                "Resp Últ. Modificação",
                "Justificativa Inativação"
            };

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cell(1, col).Value = headers[col - 1];
                worksheet.Cell(1, col).Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var usuario in usuariosPaginados.Items)
            {
                worksheet.Cell(row, 1).Value = usuario.Id.ToString();
                worksheet.Cell(row, 2).Value = usuario.Nome;
                worksheet.Cell(row, 3).Value = usuario.Email;
                worksheet.Cell(row, 4).Value = usuario.PerfilNome ?? "N/A";
                worksheet.Cell(row, 5).Value = usuario.StatusUsuario == StatusUsuarioEnum.Ativo ? "Ativo" : "Inativo";
                worksheet.Cell(row, 6).Value = usuario.DataInclusao?.ToString("dd/MM/yyyy 'às' HH:mm:ss") ?? "N/A";
                worksheet.Cell(row, 7).Value = usuario.NomeRespInclusao ?? "N/A";
                worksheet.Cell(row, 8).Value = usuario.DataUltimaAlteracao?.ToString("dd/MM/yyyy 'às' HH:mm:ss") ?? "N/A";
                worksheet.Cell(row, 9).Value = usuario.NomeRespUltimaAlteracao ?? "N/A";
                worksheet.Cell(row, 10).Value = usuario.JustificativaInativacao ?? "-";

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

    public bool Salvar()
    {
        return _usuarioRepository.Save();
    }
}