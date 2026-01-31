using DataAccess.Models;

namespace DataAccess.Interface;

public interface IBrandRepository
{
    Task<List<Brand>> GetAllAsync();
    Task<Brand?> GetByIdAsync(int id);
    Task AddAsync(Brand brand);
    Task UpdateAsync(Brand brand);
    Task DeleteAsync(Brand brand);
    Task<bool> ExistsAsync(int id);
}
