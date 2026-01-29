using System;
using System.Collections.Generic;

namespace BikeMarket.Models;

public partial class Vehicle
{
    public int Id { get; set; }

    public int SellerId { get; set; }

    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? FrameSize { get; set; }

    public string Condition { get; set; } = null!;

    public int? YearManufactured { get; set; }

    public string? Location { get; set; }

    public string? Status { get; set; }

    public string? Color { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User Seller { get; set; } = null!;

    public virtual ICollection<VehicleImage> VehicleImages { get; set; } = new List<VehicleImage>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
