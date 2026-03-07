using RepositoryPattern.Application.Contracts;

namespace RepositoryPattern.Infraestructure.Data.Core;

public class UnitOfWork(DbSessions sessions) : IUnitWork
{
    public async Task BeginAsync(CancellationToken cancellationToken = default) =>
        await sessions.BeginAsync(cancellationToken); 

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        sessions.Commit();
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        sessions.Rollback();
        return Task.CompletedTask;
    }
}