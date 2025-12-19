namespace ProductCodeApi.Models;

public class ProductCode
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}