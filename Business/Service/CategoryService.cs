using Business.Interface;
using DataAccess.Interface;
using DataAccess.Models;

namespace Business.Service;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<List<Category>> GetAllAsync()
    {
        return _categoryRepository.GetAllAsync();
    }

    public Task<Category?> GetByIdAsync(int id)
    {
        return _categoryRepository.GetByIdAsync(id);
    }

    public Task CreateAsync(Category category)
    {
        return _categoryRepository.AddAsync(category);
    }

    public Task UpdateAsync(Category category)
    {
        return _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category != null)
        {
            await _categoryRepository.DeleteAsync(category);
        }
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _categoryRepository.ExistsAsync(id);
    }
}
