using System.Collections.Generic;
using DataAccess.Models;
using DTO.Vehicle;

namespace BikeMarket.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public IEnumerable<VehicleListDTO> Vehicles { get; set; } = new List<VehicleListDTO>();
    }
}
