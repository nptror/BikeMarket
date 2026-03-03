using System.ComponentModel.DataAnnotations;

namespace BikeMarket.Models;

public class UserRatingCreateViewModel
{
    public int OrderId { get; set; }
    public int RatedUserId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string VehicleTitle { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [MaxLength(500)]
    public string? Comment { get; set; }
}
