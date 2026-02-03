using System.Collections.Generic;
using DTO.Vehicle;

namespace BikeMarket.Models
{
    public class VehicleMyPostViewModel
    {
        public int DisplayCount { get; set; }
        public int DraftCount { get; set; }
        public int PendingCount { get; set; }
        public int DeniedCount { get; set; }
        public string ActiveTab { get; set; } = "display";
        public List<VehicleListDTO> Vehicles { get; set; } = new();
    }
}
