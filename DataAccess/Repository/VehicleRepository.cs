using DataAccess.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class VehicleRepository : IVehicleRepository
{
    private readonly VehicleMarketContext _context;

    public VehicleRepository(VehicleMarketContext context)
    {
        _context = context;
    }

    public Task<List<Vehicle>> GetAllWithIncludesAsync()
    {
        return _context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Category)
            .Include(v => v.Seller)
            .ToListAsync();
    }

    public Task<List<Vehicle>> GetAvailableWithIncludesAsync()
    {
        return _context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Category)
            .Include(v => v.VehicleImages)
            .Where(v => v.Status == null || v.Status == "available")
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public Task<Vehicle?> GetByIdWithDetailsAsync(int id)
    {
        return _context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Category)
            .Include(v => v.Seller)
            .Include(v => v.VehicleImages)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public Task<Vehicle?> GetByIdWithSummaryAsync(int id)
    {
        return _context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Category)
            .Include(v => v.Seller)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public Task<Vehicle?> GetByIdWithImagesAsync(int id)
    {
        return _context.Vehicles
            .Include(v => v.VehicleImages)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public Task<Vehicle?> GetByIdAsync(int id)
    {
        return _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task AddImagesAsync(IEnumerable<VehicleImage> images)
    {
        _context.VehicleImages.AddRange(images);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Vehicles.AnyAsync(e => e.Id == id);
    }
}
