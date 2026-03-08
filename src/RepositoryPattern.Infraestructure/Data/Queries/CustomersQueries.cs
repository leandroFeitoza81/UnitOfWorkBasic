namespace RepositoryPattern.Infraestructure.Data.Queries;

public class CustomersQueries
{
    public static string GetCustomerByIdQuery => """
                                                    SELECT TOP 1 Id, Name, Email, CreatedAtUtc, IsDeleted
                                                    FROM TutoRepo.dbo.Customers WITH(READCOMMITTEDLOCK )
                                                    WHERE id = @Id AND IsDeleted = 0
                                                 """;
    
    public static string GetAllCustomers => """
                                            SELECT Id, Name, Email, CreatedAtUtc, IsDeleted
                                            FROM TutoRepo.dbo.Customers WITH(READCOMMITTEDLOCK )
                                            WHERE IsDeleted = 0
                                            ORDER BY Id
                                            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                            """;
    
    public static string InsertCustomer => """
                                           INSERT INTO TutoRepo.dbo.Customers (NAME, EMAIL, CreatedAtUtc, IsDeleted)
                                           VALUES (@Name, @Email, SYSUTCDATETIME(), 0);
                                           SELECT CAST(SCOPE_IDENTITY() AS int); 
                                           """;
    
    public static string UpdateCustomer => """
                                           UPDATE TutoRepo.dbo.Customers
                                           SET NAME = @Name, Email = @Email
                                           WHERE Id = @Id AND IsDeleted = 0
                                           """;
    
    public static string DeleteCustomer => """
                                           UPDATE TutoRepo.dbo.Customers
                                           SET IsDeleted = 1
                                           WHERE Id = @Id AND IsDeleted = 0
                                           """;
}