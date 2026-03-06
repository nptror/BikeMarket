using DataAccess.Models;

namespace DataAccess.Interface;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<List<Category>> GetAllWithVehiclesAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetByIdWithVehiclesAsync(int id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
    Task<bool> ExistsAsync(int id);
}
