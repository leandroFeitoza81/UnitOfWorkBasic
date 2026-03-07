using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RepositoryPattern.Infraestructure.Data.Abstractions;

namespace RepositoryPattern.Infraestructure.Data.Core;

public sealed class SqlConnectionFactory(IConfiguration configuration) : IConnectionFactory
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("No connection string");

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}