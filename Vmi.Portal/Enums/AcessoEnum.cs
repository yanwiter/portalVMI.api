using System;
using System.Collections.Generic;

public class AcessoConst
{
    public static readonly Guid None = Guid.Empty;
    public static readonly Guid Visualizacao = Guid.Parse("F660E8A6-7E3C-469B-A4DF-2E106ED9F546");
    public static readonly Guid Inclusao = Guid.Parse("6FC21254-DC02-4D18-8633-9349C53F305D");
    public static readonly Guid Edicao = Guid.Parse("6EF086EB-7B12-4265-A2D0-4508DB9B2D95");
    public static readonly Guid Exclusao = Guid.Parse("7A2725D3-71CD-4DC7-80A8-F8225247A619");

    private static readonly Dictionary<Guid, string> NomesFormatados = new Dictionary<Guid, string>
    {
        { None, "Nenhum" },
        { Visualizacao, "Visualização" },
        { Inclusao, "Inclusão" },
        { Edicao, "Edição" },
        { Exclusao, "Exclusão" }
    };

    public static string ObterNomePorGuid(Guid valor)
    {
        if (NomesFormatados.TryGetValue(valor, out var nomeFormatado))
        {
            return nomeFormatado;
        }

        return "Nenhum";
    }
}
