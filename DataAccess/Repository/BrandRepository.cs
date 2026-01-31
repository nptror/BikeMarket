using DataAccess.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class BrandRepository : IBrandRepository
{
    private readonly VehicleMarketContext _context;

    public BrandRepository(VehicleMarketContext context)
    {
        _context = context;
    }

    public Task<List<Brand>> GetAllAsync()
    {
        return _context.Brands.ToListAsync();
    }

    public Task<Brand?> GetByIdAsync(int id)
    {
        return _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Brand brand)
    {
        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Brands.AnyAsync(e => e.Id == id);
    }
}
