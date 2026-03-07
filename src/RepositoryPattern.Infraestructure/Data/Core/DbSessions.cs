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
    
    public void Commit() => Transaction?.Commit();
    public void Rollback() => Transaction?.Rollback();
    
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