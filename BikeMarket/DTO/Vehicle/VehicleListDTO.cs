using System;
using System.ComponentModel.DataAnnotations;

namespace BikeMarket.DTO.Vehicle
{
    public class VehicleListDTO
    {
        public int VehicleId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; } = null!;

        [Display(Name = "Brand")]
        public string BrandName { get; set; } = null!;

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Frame Size")]
        public string? FrameSize { get; set; }

        [Display(Name = "Condition")]
        public string Condition { get; set; } = null!;

        [Display(Name = "Color")]
        public string? Color { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }
        public bool IsWishlisted { get; set; }


        // Ảnh đại diện
        public string? ThumbnailUrl { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; } // available/sold/hidden...

        [Display(Name = "Posted At")]
        public DateTime? CreatedAt { get; set; }
    }
}
