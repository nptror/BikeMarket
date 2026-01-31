using DataAccess.Models;

namespace DataAccess.Interface;

public interface IWishlistRepository
{
    Task<List<Wishlist>> GetAllWithIncludesAsync();
    Task<Wishlist?> GetByIdWithIncludesAsync(int id);
    Task<Wishlist?> GetByIdAsync(int id);
    Task<Wishlist?> GetByBuyerVehicleAsync(int buyerId, int vehicleId);
    Task AddAsync(Wishlist wishlist);
    Task UpdateAsync(Wishlist wishlist);
    Task DeleteAsync(Wishlist wishlist);
    Task<bool> ExistsAsync(int id);
}
