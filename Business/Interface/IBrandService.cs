using DataAccess.Models;

namespace Business.Interface;

public interface IBrandService
{
    Task<List<Brand>> GetAllAsync();
    Task<List<Brand>> GetAllWithVehiclesAsync();
    Task<Brand?> GetByIdAsync(int id);
    Task<Brand?> GetByIdWithVehiclesAsync(int id);
    Task CreateAsync(Brand brand);
    Task UpdateAsync(Brand brand);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
