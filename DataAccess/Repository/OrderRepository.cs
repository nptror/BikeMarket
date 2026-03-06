using DataAccess.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly VehicleMarketContext _context;

    public OrderRepository(VehicleMarketContext context)
    {
        _context = context;
    }

    public Task<List<Order>> GetAllAsync()
    {
        return _context.Orders.ToListAsync();
    }

    public Task<List<Order>> GetAllWithIncludesAsync()
    {
        return _context.Orders
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .Include(o => o.Vehicle)
            .ToListAsync();
    }

    public Task<Order?> GetByIdWithIncludesAsync(int id)
    {
        return _context.Orders
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .Include(o => o.Vehicle)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        return _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public Task<Order?> GetLatestPaidOrderAsync(int buyerId, int sellerId)
    {
        return _context.Orders
            .Include(o => o.Vehicle)
            .Where(o => o.BuyerId == buyerId && o.SellerId == sellerId && o.PaymentStatus == "paid")
            .OrderByDescending(o => o.UpdatedAt ?? o.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }

    public Task<List<Order>> GetPaidOrdersByBuyerAsync(int buyerId)
    {
        return _context.Orders
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .Include(o => o.Vehicle)
                .ThenInclude(v => v.VehicleImages)
            .Where(o => o.BuyerId == buyerId && o.PaymentStatus == "paid" && o.Status != "completed")
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public Task<List<Order>> GetPaidOrdersBySellerAsync(int sellerId)
    {
        return _context.Orders
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .Include(o => o.Vehicle)
                .ThenInclude(v => v.VehicleImages)
            .Where(o => o.SellerId == sellerId && o.PaymentStatus == "paid" && o.Status != "completed")
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Orders.AnyAsync(e => e.Id == id);
    }
}
