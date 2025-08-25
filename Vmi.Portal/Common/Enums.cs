namespace Vmi.Portal.Common;

public class Enums
{
    public enum StatusCadastroEnum
    {
        ATIVO = 1,
        INATIVO = 0,
    }
    public enum TipoClienteFornecedorEnum
    {
        CLIENTE = 0,
        FORNECEDOR = 1
    }

    public enum TipoPessoaEnum
    {
        FISICA = 0,
        JURIDICA = 1
    }

    public enum RegimeTributarioEnum
    {
        SIMPLES_NACIONAL = 0,
        SIMPLES_NACIONAL_EXCESSO_SUBLIMITE = 1,
        LUCRO_PRESUMIDO = 2,
        LUCRO_REAL = 3,
        MEI = 4,
        IMUNE_ISENTO = 5,
        NAO_APLICAVEL = 6
    }

    public enum TipoEnderecoEnum
    {
        RESIDENCIAL = 0,
        COMERCIAL = 1,
        COBRANCA = 2,
        ENTREGA = 3
    }

    public enum CondicaoPagamentoEnum
    {
        AVISTA = 0,
        SETE_DIAS = 7,
        QUATORZE_DIAS = 14,
        VINTE_E_UM_DIAS = 21,
        TRINTA_DIAS = 30,
        QUARENTA_E_CINCO_DIAS = 45,
        SESSENTA_DIA = 60
    }

    public enum BancosEnum
    {
        BANCO_DO_BRASIL = 1,
        BRADESCO = 237,
        SANTANDER = 33,
        CAIXA_ECONOMICA = 104,
        ITAU = 341,
        NUBANK = 260,
        INTER = 77
    }

    public enum TipoContaEnum
    {
        CORRENTE = 0,
        POUPANCA = 1,
        SALARIO = 2,
        DIGITAL = 3,
    }

    public enum NaturezaJuridicaEnum
    {
        ORGAO_PUBLICO = 1015,
        AUTARQUIA = 1023,
        EMPRESA_PUBLICA = 1031,
        SOCIEDADE_ECONOMIA_MISTA = 1040,
        SOCIEDADE_ANONIMA = 2054,
        SOCIEDADE_LIMITADA = 2062,
        SOCIEDADE_COMANDITA_SIMPLES = 2070,
        SOCIEDADE_COMANDITA_ACOES = 2089,
        SOCIEDADE_NOME_COLETIVO = 2097,
        EMPRESARIO_INDIVIDUAL = 4086,
        MEI = 4094,
        EIRELI = 2135,
        ASSOCIACAO = 3034,
        FUNDACAO = 3042,
        ORGANIZACAO_RELIGIOSA = 3050,
        COOPERATIVA = 5070,
        CONSORCIO = 5089,
        OUTRAS = 9999
    }

    public enum PorteEmpresaEnum
    {
        MEI = 0,
        ME = 1,
        EPP = 2,
        EM = 3,
        GE = 4
    }

    public enum TipoEmpresaEnum
    {
        MATRIZ = 0,
        FILIAL = 1
    }

}
