using DataAccess.Models;

namespace Business.Interface;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<List<Category>> GetAllWithVehiclesAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetByIdWithVehiclesAsync(int id);
    Task CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
