using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BikeMarket.Dtos.Vehicle
{
    public class VehicleDetailDTO
    {
        public int VehicleId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; } = null!;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        // ===== Brand =====
        public int BrandId { get; set; }

        [Display(Name = "Brand")]
        public string BrandName { get; set; } = null!;

        // ===== Category =====
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;

        // ===== Price =====
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        // ===== Bike specific fields =====
        [Display(Name = "Frame Size")]
        public string? FrameSize { get; set; }

        [Display(Name = "Condition")]
        public string Condition { get; set; } = null!;

        [Display(Name = "Year Manufactured")]
        public int? YearManufactured { get; set; }

        [Display(Name = "Color")]
        public string? Color { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

        // ===== Seller =====
        public int SellerId { get; set; }

        [Display(Name = "Seller")]
        public string SellerName { get; set; } = null!;

        [Display(Name = "Posted At")]
        public DateTime? CreatedAt { get; set; }

        // ===== Images =====
        public List<string> ImageUrls { get; set; } = new();
    }
}
