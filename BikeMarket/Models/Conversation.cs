using System;
using System.Collections.Generic;

namespace BikeMarket.Models;

public partial class Conversation
{
    public int Id { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public int? LastMessageId { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public int BuyerUnreadCount { get; set; }

    public int SellerUnreadCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Buyer { get; set; } = null!;

    public virtual Message? LastMessage { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User Seller { get; set; } = null!;
}
