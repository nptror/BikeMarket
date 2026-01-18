using System;
using System.Collections.Generic;

namespace BikeMarket.Models;

public partial class VehicleImage
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsThumbnail { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? UploadedAt { get; set; }

    public int? FileSize { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
