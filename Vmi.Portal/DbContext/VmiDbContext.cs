using System.Data;
using System.Data.SqlClient;

namespace Vmi.Portal.DbContext;

public class VmiDbContext : BaseDbContext
{
    public VmiDbContext(string connectionName)
    {
        if (string.IsNullOrWhiteSpace(connectionName))
        {
            throw new ArgumentException("Nome da conexao inválida.", nameof(connectionName));
        }
        _connection = new Lazy<SqlConnection>(() => new SqlConnection(connectionName));
    }
    public IDbConnection Connection => _connection.Value;
}