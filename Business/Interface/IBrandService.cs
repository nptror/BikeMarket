using DataAccess.Models;

namespace Business.Interface;

public interface IBrandService
{
    Task<List<Brand>> GetAllAsync();
    Task<Brand?> GetByIdAsync(int id);
    Task CreateAsync(Brand brand);
    Task UpdateAsync(Brand brand);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
