using DataAccess.Models;
using DTO.Vehicle;
using Microsoft.AspNetCore.Http;

namespace Business.Interface;

public interface IVehicleService
{
    Task<List<Vehicle>> GetAllAsync();
    Task<VehicleDetailDTO?> GetDetailAdminAsync(int id);
    Task<VehicleDetailDTO?> GetDetailBuyerAsync(int id, int? currentUserId);
    Task<Vehicle?> GetForEditAsync(int id);
    Task<Vehicle?> GetForDeleteAsync(int id);
    Task CreateAsync(Vehicle vehicle, List<IFormFile>? images, int sellerId);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<List<Brand>> GetBrandsAsync();
    Task<List<Category>> GetCategoriesAsync();
    Task<List<User>> GetSellersAsync();
    Task<List<VehicleListDTO>> GetAvailableListAsync();
}
