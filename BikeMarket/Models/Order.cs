using System;
using System.Collections.Generic;

namespace BikeMarket.Models;

public partial class Order
{
    public int Id { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public int VehicleId { get; set; }

    public string? Status { get; set; }

    public decimal TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Buyer { get; set; } = null!;

    public virtual User Seller { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
