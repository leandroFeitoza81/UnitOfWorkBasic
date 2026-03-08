using System.Data;
using RepositoryPattern.Infraestructure.Data.Abstractions;

namespace RepositoryPattern.Infraestructure.Data.Core;

public class DbSessions : IDisposable, IAsyncDisposable
{
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; private set; }

    public DbSessions(IConnectionFactory connection)
    {
        Connection = connection.CreateConnection();
        if (Connection.State != ConnectionState.Open)
            Connection.Open();
    }

    public void Begin()
    {
        Transaction ??= Connection.BeginTransaction();
    }

    public Task BeginAsync(CancellationToken cancellationToken = default)
    {
        Begin();
        return Task.CompletedTask;
    }

    public void Commit()
    {
        if (Transaction is null)
            return;
        
        Transaction.Commit();
        Transaction.Dispose();
        Transaction = null;
    }

    public void Rollback()
    {
        if (Transaction is null)
            return;
        
        Transaction.Rollback();
        Transaction.Dispose();
        Transaction = null;
    }
    
    public void Dispose()
    {
        Transaction?.Dispose();
        Connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        Transaction?.Dispose();
        Connection.Dispose();
        await Task.CompletedTask;
    }
}