using DataAccess.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly VehicleMarketContext _context;

    public CategoryRepository(VehicleMarketContext context)
    {
        _context = context;
    }

    public Task<List<Category>> GetAllAsync()
    {
        return _context.Categories.ToListAsync();
    }

    public Task<List<Category>> GetAllWithVehiclesAsync()
    {
        return _context.Categories.Include(c => c.Vehicles).ToListAsync();
    }

    public Task<Category?> GetByIdAsync(int id)
    {
        return _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<Category?> GetByIdWithVehiclesAsync(int id)
    {
        return _context.Categories.Include(c => c.Vehicles).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        var existing = await _context.Categories.FindAsync(category.Id);
        if (existing != null)
        {
            existing.Name = category.Name;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Categories.AnyAsync(c => c.Id == id);
    }
}
