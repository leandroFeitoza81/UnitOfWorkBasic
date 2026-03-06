namespace RepositoryPattern.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}