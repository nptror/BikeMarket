using Business.Interface;
using DataAccess.Interface;
using DataAccess.Models;

namespace Business.Service;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public Task<List<Brand>> GetAllAsync()
    {
        return _brandRepository.GetAllAsync();
    }

    public Task<Brand?> GetByIdAsync(int id)
    {
        return _brandRepository.GetByIdAsync(id);
    }

    public Task CreateAsync(Brand brand)
    {
        return _brandRepository.AddAsync(brand);
    }

    public Task UpdateAsync(Brand brand)
    {
        return _brandRepository.UpdateAsync(brand);
    }

    public async Task DeleteAsync(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand != null)
        {
            await _brandRepository.DeleteAsync(brand);
        }
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _brandRepository.ExistsAsync(id);
    }
}
