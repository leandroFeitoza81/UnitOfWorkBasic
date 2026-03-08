using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using RepositoryPattern.Application.Contracts;
using RepositoryPattern.Domain.Entities;
using RepositoryPattern.Infraestructure.Data.Core;
using RepositoryPattern.Infraestructure.Data.Queries;
using RepositoryPattern.Infraestructure.Data.Utils;

namespace RepositoryPattern.Infraestructure.Data.Repositories;

public class CustomerRepository(DbSessions session) : IRepository<Customer>
{
    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var command = CreateCommand(CustomersQueries.GetCustomerByIdQuery);
        AddParameter(command, "@Id", SqlDbType.Int, id);

        await using var reader = await ((DbCommand)command).ExecuteReaderAsync(cancellationToken);
        
        if (await reader.ReadAsync(cancellationToken))
            return MapToCustomer(reader);

        return null;
    }
    
    public async Task<IReadOnlyList<Customer>> ListAsync(int skip = 0, int take = 10,
        CancellationToken cancellationToken = default)
    {
        using var command = CreateCommand(CustomersQueries.GetAllCustomers);
        AddParameter(command, "@Skip", SqlDbType.Int, skip);
        AddParameter(command, "@Take", SqlDbType.Int, take);
        
        var list = new List<Customer>(take);
        await using var reader = await ((DbCommand)command).ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            list.Add(MapToCustomer(reader));
            
        return list;
    }

    public async Task<int> AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        using var command = CreateCommand(CustomersQueries.InsertCustomer);
        AddParameter(command, "@Name", SqlDbType.NVarChar, entity.Name, 200);
        AddParameter(command, "@Email", SqlDbType.NVarChar, entity.Email);
        
        var id = await ((DbCommand)command).ExecuteScalarAsync(cancellationToken);
        entity.Id = Convert.ToInt32(id);
        entity.CreatedAtUtc = DateTime.UtcNow;
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        using var command = CreateCommand(CustomersQueries.UpdateCustomer);
        AddParameter(command, "@Id", SqlDbType.Int, entity.Id);
        AddParameter(command, "@Name", SqlDbType.NVarChar, entity.Name, 200);
        AddParameter(command, "@Email", SqlDbType.NVarChar, (object?)entity.Email ?? DBNull.Value, 320);
        
        var rows = await ((DbCommand)command).ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        using var command = CreateCommand(CustomersQueries.DeleteCustomer);
        AddParameter(command, "@Id", SqlDbType.Int, id);
        var rows = await ((DbCommand)command).ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
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

