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
}
