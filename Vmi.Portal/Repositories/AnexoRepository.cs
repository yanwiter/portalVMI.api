using Dapper;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class AnexoRepository: IAnexoRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public AnexoRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<Anexo> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT * FROM Anexos 
            WHERE Id = @Id";

        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<Anexo>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Anexo>> GetAllAsync()
    {
        const string sql = @"
            SELECT * FROM Anexos";

        return await _vmiDbContext.Connection.QueryAsync<Anexo>(sql);
    }

    public async Task<Anexo> AddAsync(Anexo anexo)
    {
        const string sql = @"
            INSERT INTO Anexos 
                (Nome, Conteudo, Extensao, IdRespInclusao, DataInclusao, NomeRespInclusao)
            VALUES 
                (@Nome, @Conteudo, @Extensao, @IdRespInclusao, @DataInclusao, @NomeRespInclusao);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        var id = await _vmiDbContext.Connection.ExecuteScalarAsync<Guid>(sql, anexo);
        anexo.Id = id;

        return anexo;
    }

    public async Task<IEnumerable<Anexo>> GetByIdClienteFornecedorAsync(List<Guid> idClienteFornecedor)
    {
        if (idClienteFornecedor == null || !idClienteFornecedor.Any())
        {
            return Enumerable.Empty<Anexo>();
        }

        const string sql = @"
        SELECT 
            Id,
            IdClienteFornecedor,
            Nome,
            Conteudo,
            Tamanho,
            Tipo,
            CaminhoArquivo,
            Extensao,
            IdRespInclusao,
            DataInclusao,
            NomeRespInclusao
        FROM Anexos 
        WHERE IdClienteFornecedor IN @IdClienteFornecedor";

        return await _vmiDbContext.Connection.QueryAsync<Anexo>(sql,
            new { IdClienteFornecedor = idClienteFornecedor });
    }

}
