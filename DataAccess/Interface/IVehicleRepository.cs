using DataAccess.Models;

namespace DataAccess.Interface;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllWithIncludesAsync();
    Task<List<Vehicle>> GetAvailableWithIncludesAsync();
    Task<Vehicle?> GetByIdWithDetailsAsync(int id);
    Task<Vehicle?> GetByIdWithSummaryAsync(int id);
    Task<Vehicle?> GetByIdWithImagesAsync(int id);
    Task<Vehicle?> GetByIdAsync(int id);
    Task AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
    Task AddImagesAsync(IEnumerable<VehicleImage> images);
    Task<bool> ExistsAsync(int id);
}
