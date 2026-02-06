using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class UserRating
{
    public int Id { get; set; }

    public int RaterId { get; set; }

    public int RatedUserId { get; set; }

    public int OrderId { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User RatedUser { get; set; } = null!;

    public virtual User Rater { get; set; } = null!;
}
