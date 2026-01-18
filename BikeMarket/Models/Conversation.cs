using System;
using System.Collections.Generic;

namespace BikeMarket.Models;

public partial class Conversation
{
    public int Id { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public int VehicleId { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public string? LastMessageText { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Buyer { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User Seller { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
