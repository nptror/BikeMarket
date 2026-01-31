using DataAccess.Models;

namespace DataAccess.Interface;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
}
