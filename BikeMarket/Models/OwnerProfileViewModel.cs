using System.Collections.Generic;
using DataAccess.Models;
using DTO.Vehicle;

namespace BikeMarket.Models
{
    public class OwnerProfileViewModel
    {
        public User? Seller { get; set; }
        public decimal? RatingAvg { get; set; }
        public int RatingCount { get; set; }
        public IEnumerable<UserRating> Ratings { get; set; } = new List<UserRating>();
        public Dictionary<string, int> TagCounts { get; set; } = new();
        public IEnumerable<VehicleListDTO> Vehicles { get; set; } = new List<VehicleListDTO>();
        public bool CanRateSeller { get; set; }
        public int? PaidOrderId { get; set; }
    }
}
