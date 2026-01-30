using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Wishlist
{
    public int Id { get; set; }

    public int BuyerId { get; set; }

    public int VehicleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Buyer { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
