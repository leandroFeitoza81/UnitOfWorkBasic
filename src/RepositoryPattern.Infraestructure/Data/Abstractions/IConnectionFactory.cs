using System.Data;

namespace RepositoryPattern.Infraestructure.Data.Abstractions;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}