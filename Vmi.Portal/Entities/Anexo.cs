using System;

namespace Vmi.Portal.Entities;

public class Anexo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdClienteFornecedor { get; set; }
    public string Nome { get; set; }
    public byte[] Conteudo { get; set; }
    public long Tamanho { get; set; }
    public string Tipo { get; set; }
    public string CaminhoArquivo { get; set; }
    public string Extensao { get; set; }
    public Guid IdRespInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public string NomeRespInclusao { get; set; }
}