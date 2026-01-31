using Business.Interface;
using Business.Models;
using DataAccess.Interface;
using DataAccess.Models;

namespace Business.Service;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public WishlistService(IWishlistRepository wishlistRepository, IUserRepository userRepository, IVehicleRepository vehicleRepository)
    {
        _wishlistRepository = wishlistRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
    }

    public Task<List<Wishlist>> GetAllAsync()
    {
        return _wishlistRepository.GetAllWithIncludesAsync();
    }

    public Task<Wishlist?> GetByIdAsync(int id)
    {
        return _wishlistRepository.GetByIdWithIncludesAsync(id);
    }

    public Task CreateAsync(Wishlist wishlist)
    {
        return _wishlistRepository.AddAsync(wishlist);
    }

    public Task UpdateAsync(Wishlist wishlist)
    {
        return _wishlistRepository.UpdateAsync(wishlist);
    }

    public async Task DeleteAsync(int id)
    {
        var wishlist = await _wishlistRepository.GetByIdAsync(id);
        if (wishlist != null)
        {
            await _wishlistRepository.DeleteAsync(wishlist);
        }
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _wishlistRepository.ExistsAsync(id);
    }

    public async Task<WishlistToggleResult> ToggleAsync(int userId, int vehicleId)
    {
        if (!await _vehicleRepository.ExistsAsync(vehicleId))
        {
            return new WishlistToggleResult { Success = false, ErrorMessage = "VEHICLE_NOT_FOUND" };
        }

        var existed = await _wishlistRepository.GetByBuyerVehicleAsync(userId, vehicleId);
        if (existed != null)
        {
            await _wishlistRepository.DeleteAsync(existed);
            return new WishlistToggleResult { Success = true, IsWishlisted = false };
        }

        await _wishlistRepository.AddAsync(new Wishlist
        {
            BuyerId = userId,
            VehicleId = vehicleId,
            CreatedAt = DateTime.Now
        });

        return new WishlistToggleResult { Success = true, IsWishlisted = true };
    }

    public Task<List<User>> GetUsersAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public Task<List<Vehicle>> GetVehiclesAsync()
    {
        return _vehicleRepository.GetAllWithIncludesAsync();
    }
}
