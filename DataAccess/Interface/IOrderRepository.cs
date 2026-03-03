using DataAccess.Models;

namespace DataAccess.Interface;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<List<Order>> GetAllWithIncludesAsync();
    Task<Order?> GetByIdWithIncludesAsync(int id);
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetLatestPaidOrderAsync(int buyerId, int sellerId);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
    Task<bool> ExistsAsync(int id);
}
