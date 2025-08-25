using System.Globalization;

using System.Linq;
using System.Text;
using Vmi.Portal.Common;
using static Vmi.Portal.Common.Enums;

namespace Vmi.Portal.Utils
{
    public class Util
    {
        public static DateTime PegaHoraBrasilia()
        {
            try
            {
                return TimeZoneInfo.ConvertTime(
                    DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        public static Task<DateTime> PegaHoraBrasiliaAsync()
        {
            return Task.FromResult(PegaHoraBrasilia());
        }

        public static bool ConverterDataParaDDMMYYYY(string dateStr, string format, out DateTime? result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(dateStr))
                return true;

            if (DateTime.TryParseExact(dateStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                result = parsedDate;
                return true;
            }

            return false;
        }

        public static string FormataCPFCNPJ(string cpfCnpj, int? tipoPessoa)
        {
            if (string.IsNullOrEmpty(cpfCnpj))
                return "N/A";

            var digits = new string(cpfCnpj.Where(char.IsDigit).ToArray());

            if (tipoPessoa == (int)TipoPessoaEnum.FISICA)
            {
                return digits.Length == 11 ?
                    $"{digits.Substring(0, 3)}.{digits.Substring(3, 3)}.{digits.Substring(6, 3)}-{digits.Substring(9)}" :
                    cpfCnpj;
            }
            else
            {
                return digits.Length == 14 ?
                    $"{digits.Substring(0, 2)}.{digits.Substring(2, 3)}.{digits.Substring(5, 3)}/{digits.Substring(8, 4)}-{digits.Substring(12)}" :
                    cpfCnpj;
            }
        }

        public static string FormataRG(string rg) 
        {
            if (string.IsNullOrEmpty(rg))
                return "N/A";

            string rgNumerico = new string(rg.Where(char.IsDigit).ToArray());

            rgNumerico = rgNumerico.PadLeft(9, '0');
            string rgFormatado = $"{rgNumerico.Substring(0, 2)}.{rgNumerico.Substring(2, 3)}.{rgNumerico.Substring(5, 3)}-{rgNumerico.Substring(8)}";

            return rgFormatado;
        }


        public static string FormataCNAE(string cnae)
        {
            if (string.IsNullOrEmpty(cnae))
                return "N/A";

            var digits = new string(cnae.Where(char.IsDigit).ToArray());

            return digits.Length == 7 ?
                $"{digits.Substring(0, 4)}-{digits.Substring(4, 1)}/{digits.Substring(5)}" :
                cnae;
        }

        public static string FormatCEP(string cep)
        {
            if (string.IsNullOrEmpty(cep))
                return "N/A";

            var digits = new string(cep.Where(char.IsDigit).ToArray());

            return digits.Length == 8 ?
                $"{digits.Substring(0, 5)}-{digits.Substring(5)}" :
                cep;
        }

        public static string GetTipoCadastroDescription(int? tipoCadastro)
        {
            if (tipoCadastro == null) return "N/A";

            return tipoCadastro switch
            {
                (int)TipoClienteFornecedorEnum.CLIENTE => "Cliente",
                (int)TipoClienteFornecedorEnum.FORNECEDOR => "Fornecedor",
                _ => "N/A"
            };
        }

        public static string GetTipoPessoaDescription(int? tipoPessoa)
        {
            if (tipoPessoa == null) return "N/A";

            return tipoPessoa switch
            {
                (int)TipoPessoaEnum.FISICA => "Física",
                (int)TipoPessoaEnum.JURIDICA => "Jurídica",
                _ => "N/A"
            };
        }

        public static string GetTipoEmpresaDescription(int? tipoEmpresa)
        {
            if (tipoEmpresa == null) return "N/A";

            return tipoEmpresa switch
            {
                (int)TipoEmpresaEnum.MATRIZ => "Matriz",
                (int)TipoEmpresaEnum.FILIAL => "Filial",
                _ => "N/A"
            };
        }

        public static string GetPorteEmpresaDescription(int? porteEmpresa)
        {
            if (porteEmpresa == null) return "N/A";

            return porteEmpresa switch
            {
                (int)PorteEmpresaEnum.MEI => "Microempreendedor Individual",
                (int)PorteEmpresaEnum.ME => "Microempresa",
                (int)PorteEmpresaEnum.EPP => "Empresa de Pequeno Porte",
                (int)PorteEmpresaEnum.EM => "Média Empresa",
                (int)PorteEmpresaEnum.GE => "Grande Empresa",
                _ => "N/A"
            };
        }

        public static string GetNaturezaJuridicaDescription(int? naturezaJuridica)
        {
            if (naturezaJuridica == null) return "N/A";

            return naturezaJuridica switch
            {
                (int)NaturezaJuridicaEnum.ORGAO_PUBLICO => "Órgão Público",
                (int)NaturezaJuridicaEnum.AUTARQUIA => "Autarquia",
                (int)NaturezaJuridicaEnum.EMPRESA_PUBLICA => "Empresa Pública",
                (int)NaturezaJuridicaEnum.SOCIEDADE_ECONOMIA_MISTA => "Sociedade de Economia Mista",
                (int)NaturezaJuridicaEnum.SOCIEDADE_ANONIMA => "Sociedade Anônima",
                (int)NaturezaJuridicaEnum.SOCIEDADE_LIMITADA => "Sociedade Limitada",
                (int)NaturezaJuridicaEnum.SOCIEDADE_COMANDITA_SIMPLES => "Sociedade em Comandita Simples",
                (int)NaturezaJuridicaEnum.SOCIEDADE_COMANDITA_ACOES => "Sociedade em Comandita por Ações",
                (int)NaturezaJuridicaEnum.SOCIEDADE_NOME_COLETIVO => "Sociedade em Nome Coletivo",
                (int)NaturezaJuridicaEnum.EMPRESARIO_INDIVIDUAL => "Empresário Individual",
                (int)NaturezaJuridicaEnum.MEI => "MEI",
                (int)NaturezaJuridicaEnum.EIRELI => "EIRELI",
                (int)NaturezaJuridicaEnum.ASSOCIACAO => "Associação",
                (int)NaturezaJuridicaEnum.FUNDACAO => "Fundação",
                (int)NaturezaJuridicaEnum.ORGANIZACAO_RELIGIOSA => "Organização Religiosa",
                (int)NaturezaJuridicaEnum.COOPERATIVA => "Cooperativa",
                (int)NaturezaJuridicaEnum.CONSORCIO => "Consórcio",
                (int)NaturezaJuridicaEnum.OUTRAS => "Outras",
                _ => "N/A"
            };
        }

        public static string GetRegimeTributarioDescription(int? regimeTributario)
        {
            if (regimeTributario == null) return "N/A";

            return regimeTributario switch
            {
                (int)RegimeTributarioEnum.SIMPLES_NACIONAL => "Simples Nacional",
                (int)RegimeTributarioEnum.SIMPLES_NACIONAL_EXCESSO_SUBLIMITE => "Simples Nacional (Excesso Sublimite)",
                (int)RegimeTributarioEnum.LUCRO_PRESUMIDO => "Lucro Presumido",
                (int)RegimeTributarioEnum.LUCRO_REAL => "Lucro Real",
                (int)RegimeTributarioEnum.MEI => "MEI",
                (int)RegimeTributarioEnum.IMUNE_ISENTO => "Imune/Isento",
                (int)RegimeTributarioEnum.NAO_APLICAVEL => "Não Aplicável",
                _ => "N/A"
            };
        }

        public static string FormatTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
                return "N/A";

            var digits = new string(telefone.Where(char.IsDigit).ToArray());

            if (digits.Length == 10)
            {
                return $"({digits.Substring(0, 2)}) {digits.Substring(2, 4)}-{digits.Substring(6)}";
            }
            else if (digits.Length == 11)
            {
                return $"({digits.Substring(0, 2)}) {digits.Substring(2, 5)}-{digits.Substring(7)}";
            }
            else if (digits.Length == 8)
            {
                return $"{digits.Substring(0, 4)}-{digits.Substring(4)}";
            }
            else if (digits.Length == 9)
            {
                return $"{digits.Substring(0, 5)}-{digits.Substring(5)}";
            }

            return telefone;
        }

        public static string GetTipoEnderecoDescription(int? tipoEndereco)
        {
            if (tipoEndereco == null) return "N/A";

            return tipoEndereco switch
            {
                (int)TipoEnderecoEnum.RESIDENCIAL => "Residencial",
                (int)TipoEnderecoEnum.COMERCIAL => "Comercial",
                (int)TipoEnderecoEnum.COBRANCA => "Cobrança",
                (int)TipoEnderecoEnum.ENTREGA => "Entrega",
                _ => "N/A"
            };
        }
    }
}