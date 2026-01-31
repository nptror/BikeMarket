using Business.Models;
using DataAccess.Models;

namespace Business.Interface;

public interface IWishlistService
{
    Task<List<Wishlist>> GetAllAsync();
    Task<Wishlist?> GetByIdAsync(int id);
    Task CreateAsync(Wishlist wishlist);
    Task UpdateAsync(Wishlist wishlist);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<WishlistToggleResult> ToggleAsync(int userId, int vehicleId);
    Task<List<User>> GetUsersAsync();
    Task<List<Vehicle>> GetVehiclesAsync();
}
