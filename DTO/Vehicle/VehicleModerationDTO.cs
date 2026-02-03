namespace DTO.Vehicle;

public class VehicleModerationDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string SellerName { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? MainImageUrl { get; set; }
    public string? Location { get; set; }
}
