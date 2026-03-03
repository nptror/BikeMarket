using DataAccess.Models;

namespace BikeMarket.Models;

public class OrderPaymentSuccessViewModel
{
    public Order Order { get; set; } = null!;
    public bool CanRateSeller { get; set; }
}
