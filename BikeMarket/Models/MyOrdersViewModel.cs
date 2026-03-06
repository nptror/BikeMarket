using DataAccess.Models;

namespace BikeMarket.Models;

public class MyOrdersViewModel
{
    public string ActiveTab { get; set; } = "bought";
    public List<Order> BoughtOrders { get; set; } = new();
    public List<Order> HandoverOrders { get; set; } = new();
    public int BoughtCount { get; set; }
    public int HandoverCount { get; set; }
}
