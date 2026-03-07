using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using RepositoryPattern.Application.Contracts;
using RepositoryPattern.Domain.Entities;
using RepositoryPattern.Infraestructure.Data.Core;
using RepositoryPattern.Infraestructure.Data.Utils;

namespace RepositoryPattern.Infraestructure.Data.Repositories;

public class CustomerRepository(DbSessions session) : IRepository<Customer>
{
    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var command = CreateCommand("""
                                          SELECT TOP 1 Id, Name, Email, CreatedAtUtc, IsDeleted
                                          FROM dbo.Customer WITH(NOLOCK)
                                          WHERE id = @id AND IsDeleted = 0
                                          """);
        
        AddParameter(command, "@Id", SqlDbType.Int, id);

        await using var reader = await ((DbCommand)command).ExecuteReaderAsync(cancellationToken);
        
        if (await reader.ReadAsync(cancellationToken))
            return MapToCustomer(reader);

        return null;
    }
    
    public Task<IReadOnlyList<Customer>> ListAsync(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    private IDisposable CreateCommand(string query, CommandType commandType = CommandType.Text)
    {
        var command = session.Connection.CreateCommand();
        command.Transaction = session.Transaction;
        command.CommandText = query;
        command.CommandType = commandType;
        command.CommandTimeout = 30;
        return command;
    }

    private void AddParameter(IDisposable command, string name, SqlDbType type, object? value, int? size = null)
    {
        var parameter = new SqlParameter(name, type)
        {
            Value = value ?? DBNull.Value,
        };
        
        if (size.HasValue)
            parameter.Size = size.Value;
        
        ((SqlCommand)command).Parameters.Add(parameter);
    }

    private static Customer MapToCustomer(IDataRecord record)
    {
        return new Customer()
        {
            Id = record.GetInt("Id"),
            Name = record.GetString("Name") ?? string.Empty,
            Email = record.GetString("Email"),
            CreatedAtUtc = record.GetDateTime("CreatedAtUtc"),
            IsDeleted = record.GetBoolean("IsDeleted"),
        };
    }

}

