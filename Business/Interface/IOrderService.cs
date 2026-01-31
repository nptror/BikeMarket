using DataAccess.Models;

namespace Business.Interface;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task CreateAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<Order?> BuyNowAsync(int buyerId, int vehicleId);
    Task<bool> PayAsync(int id);
    Task<List<User>> GetUsersAsync();
    Task<List<Vehicle>> GetVehiclesAsync();
}
