using System.ComponentModel;

namespace Vmi.Portal.Enums;

public enum AcessoEnum
{
    [Description("Nenhum")]
    None = 0,

    [Description("Visualização")]
    Visualizacao = 1,

    [Description("Inclusão")]
    Inclusao = 2,

    [Description("Edição")]
    Edicao = 3,

    [Description("Exclusão")]
    Exclusao = 4
}