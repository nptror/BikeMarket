using DataAccess.Models;

namespace DataAccess.Interface;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<List<User>> GetAllAsync(
        string? search = null,
        decimal ratingAvg = 0,
        string? role = null,
        string? sortBy = "email",
        string? sortOrder = "asc");
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<bool> ExistsAsync(int id);
}
