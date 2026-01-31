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

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Orders.AnyAsync(e => e.Id == id);
    }
}
