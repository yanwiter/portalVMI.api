using Dapper;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class AcessoRepository : IAcessoRepository
{
    private readonly VmiDbContext _vmiDbContext;
    public AcessoRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task AdicionarAcesso(Acesso acesso)
    {
        string sql = @"
                INSERT INTO
                    Acessos (Nome, Descricao)
                VALUES
                    (@Nome, @Descricao)";
        _vmiDbContext.Connection.Execute(sql, acesso);
    }

    public async Task AtualizarAcesso(Acesso acesso)
    {
        string sql = @"
                UPDATE
                    Acessos
                SET
                    Nome = @Nome,
                    Descricao = @Descricao
                WHERE
                    Id = @Id";
        _vmiDbContext.Connection.Execute(sql, acesso);
        
    }

    public Acesso ObterAcessoPorId(int id)
    {
        string sql = "SELECT * FROM Acessos WHERE Id = @Id";
        return _vmiDbContext.Connection.QueryFirstOrDefault<Acesso>(sql, new { Id = id });
    }

    public IEnumerable<Acesso> ObterTodosAcessos()
    {
        string sql = "SELECT * FROM Acessos";
        return _vmiDbContext.Connection.Query<Acesso>(sql);
    }

    public async Task RemoverAcesso(int id)
    {
        string sql = "DELETE FROM Acessos WHERE Id = @Id";
        _vmiDbContext.Connection.Execute(sql, new { Id = id });
    }

    public bool Save()
    {
        return true;
    }
}
