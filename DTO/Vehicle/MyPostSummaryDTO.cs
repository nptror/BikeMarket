using System.Collections.Generic;

namespace DTO.Vehicle
{
    public class MyPostSummaryDTO
    {
        public int DisplayCount { get; set; }
        public int DraftCount { get; set; }
        public int PendingCount { get; set; }
        public int DeniedCount { get; set; }
        public List<VehicleListDTO> Vehicles { get; set; } = new();
    }
}
