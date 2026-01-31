using DataAccess.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class WishlistRepository : IWishlistRepository
{
    private readonly VehicleMarketContext _context;

    public WishlistRepository(VehicleMarketContext context)
    {
        _context = context;
    }

    public Task<List<Wishlist>> GetAllWithIncludesAsync()
    {
        return _context.Wishlists
            .Include(w => w.Buyer)
            .Include(w => w.Vehicle)
            .ToListAsync();
    }

    public Task<Wishlist?> GetByIdWithIncludesAsync(int id)
    {
        return _context.Wishlists
            .Include(w => w.Buyer)
            .Include(w => w.Vehicle)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public Task<Wishlist?> GetByIdAsync(int id)
    {
        return _context.Wishlists.FirstOrDefaultAsync(w => w.Id == id);
    }

    public Task<Wishlist?> GetByBuyerVehicleAsync(int buyerId, int vehicleId)
    {
        return _context.Wishlists.FirstOrDefaultAsync(w => w.BuyerId == buyerId && w.VehicleId == vehicleId);
    }

    public async Task AddAsync(Wishlist wishlist)
    {
        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Wishlist wishlist)
    {
        _context.Wishlists.Update(wishlist);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Wishlist wishlist)
    {
        _context.Wishlists.Remove(wishlist);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Wishlists.AnyAsync(e => e.Id == id);
    }
}
