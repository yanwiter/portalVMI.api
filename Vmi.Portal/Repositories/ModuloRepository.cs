using Dapper;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class ModuloRepository : IModuloRepository

{
    private readonly VmiDbContext _vmiDbContext;

    public ModuloRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task AdicionarModulo(Modulo modulo)
    {
        string sql = @"
            INSERT INTO
                Modulos (Nome, Descricao)
            VALUES
                (@Nome, @Descricao);
            SELECT
                CAST(SCOPE_IDENTITY() as int)";

        var id = _vmiDbContext.Connection.QuerySingle<int>(sql, modulo);
        modulo.Id = id;
    }

    public async Task AtualizarModulo(Modulo modulo)
    {
        string sql = @"
            UPDATE 
                Modulos
            SET 
               Nome = @Nome,
               Descricao = @Descricao,
            WHERE Id = @Id";

        _vmiDbContext.Connection.Execute(sql, modulo);
    }

    public Modulo ObterModuloPorId(int id)
    {
        string sql = "SELECT * FROM Modulos WHERE Id = @Id";
        return _vmiDbContext.Connection.QueryFirstOrDefault<Modulo>(sql, new { Id = id });
    }

    public IEnumerable<Modulo> ObterTodosModulos()
    {
        string sql = "SELECT * FROM Modulos";
        return _vmiDbContext.Connection.Query<Modulo>(sql);
    }

    public async Task RemoverModulo(int id)
    {
        string sql = "DELETE FROM Modulos WHERE Id = @Id";
        _vmiDbContext.Connection.Execute(sql, new { Id = id });
    }

    public bool Save()
    {
        return true;
    }
}
