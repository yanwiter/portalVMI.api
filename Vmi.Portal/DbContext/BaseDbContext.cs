using System.Data.SqlClient;

namespace Vmi.Portal.DbContext;

public abstract class BaseDbContext : IDisposable
{
    protected Lazy<SqlConnection> _connection;

    #region IDisposable Support

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing && _connection.IsValueCreated)
            {
                _connection.Value.Dispose();
                _connection = null;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}