using Business.Interface;
using DataAccess.Interface;
using DataAccess.Models;
using DTO.Vehicle;
using Microsoft.AspNetCore.Http;

namespace Business.Service;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IPhotoService _photoService;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IWishlistRepository wishlistRepository,
        IPhotoService photoService)
    {
        _vehicleRepository = vehicleRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _wishlistRepository = wishlistRepository;
        _photoService = photoService;
    }

    public Task<List<Vehicle>> GetAllAsync()
    {
        return _vehicleRepository.GetAllWithIncludesAsync();
    }

    public async Task<VehicleDetailDTO?> GetDetailAdminAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
        if (vehicle == null)
        {
            return null;
        }

        return new VehicleDetailDTO
        {
            VehicleId = vehicle.Id,
            Title = vehicle.Title,
            Description = vehicle.Description,
            BrandId = vehicle.BrandId,
            BrandName = vehicle.Brand?.Name ?? "Unknown",
            CategoryId = vehicle.CategoryId,
            CategoryName = vehicle.Category?.Name ?? "Unknown",
            Price = vehicle.Price,
            FrameSize = vehicle.FrameSize,
            Condition = vehicle.Condition,
            YearManufactured = vehicle.YearManufactured,
            Color = vehicle.Color,
            Location = vehicle.Location,
            Status = vehicle.Status,
            SellerId = vehicle.SellerId,
            SellerName = vehicle.Seller?.Name ?? "Unknown",
            CreatedAt = vehicle.CreatedAt,
            ImageUrls = vehicle.VehicleImages.Select(img => img.ImageUrl).ToList()
        };
    }

    public async Task<VehicleDetailDTO?> GetDetailBuyerAsync(int id, int? currentUserId)
    {
        var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
        if (vehicle == null)
        {
            return null;
        }

        var isWishlisted = false;
        if (currentUserId != null)
        {
            isWishlisted = await _wishlistRepository.GetByBuyerVehicleAsync(currentUserId.Value, vehicle.Id) != null;
        }

        return new VehicleDetailDTO
        {
            VehicleId = vehicle.Id,
            Title = vehicle.Title,
            Description = vehicle.Description,
            BrandId = vehicle.BrandId,
            BrandName = vehicle.Brand?.Name ?? "Unknown",
            CategoryId = vehicle.CategoryId,
            CategoryName = vehicle.Category?.Name ?? "Unknown",
            Price = vehicle.Price,
            FrameSize = vehicle.FrameSize,
            Condition = vehicle.Condition,
            YearManufactured = vehicle.YearManufactured,
            Color = vehicle.Color,
            Location = vehicle.Location,
            Status = vehicle.Status,
            IsWishlisted = isWishlisted,
            SellerId = vehicle.SellerId,
            SellerName = vehicle.Seller?.Name ?? "Unknown",
            CreatedAt = vehicle.CreatedAt,
            ImageUrls = vehicle.VehicleImages.Select(img => img.ImageUrl).ToList()
        };
    }

    public Task<Vehicle?> GetForEditAsync(int id)
    {
        return _vehicleRepository.GetByIdWithImagesAsync(id);
    }

    public Task<Vehicle?> GetForDeleteAsync(int id)
    {
        return _vehicleRepository.GetByIdWithSummaryAsync(id);
    }

    public async Task CreateAsync(Vehicle vehicle, List<IFormFile>? images, int sellerId)
    {
        vehicle.SellerId = sellerId;
        vehicle.CreatedAt = DateTime.Now;
        vehicle.Status = string.IsNullOrEmpty(vehicle.Status) ? "available" : vehicle.Status;

        await _vehicleRepository.AddAsync(vehicle);

        if (images != null && images.Count > 0)
        {
            var vehicleImages = new List<VehicleImage>();
            foreach (var file in images)
            {
                var imageUrl = await _photoService.UploadImageAsync(file);
                vehicleImages.Add(new VehicleImage
                {
                    VehicleId = vehicle.Id,
                    ImageUrl = imageUrl
                });
            }

            await _vehicleRepository.AddImagesAsync(vehicleImages);
        }
    }

    public Task UpdateAsync(Vehicle vehicle)
    {
        return _vehicleRepository.UpdateAsync(vehicle);
    }

    public async Task DeleteAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle != null)
        {
            await _vehicleRepository.DeleteAsync(vehicle);
        }
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _vehicleRepository.ExistsAsync(id);
    }

    public Task<List<Brand>> GetBrandsAsync()
    {
        return _brandRepository.GetAllAsync();
    }

    public Task<List<Category>> GetCategoriesAsync()
    {
        return _categoryRepository.GetAllAsync();
    }

    public Task<List<User>> GetSellersAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public async Task<List<VehicleListDTO>> GetAvailableListAsync()
    {
        var vehicles = await _vehicleRepository.GetAvailableWithIncludesAsync();
        return vehicles.Select(v => new VehicleListDTO
        {
            VehicleId = v.Id,
            Title = v.Title,
            BrandName = v.Brand.Name,
            CategoryName = v.Category.Name,
            Price = v.Price,
            FrameSize = v.FrameSize,
            Condition = v.Condition,
            Color = v.Color,
            Location = v.Location,
            Status = v.Status,
            CreatedAt = v.CreatedAt,
            ThumbnailUrl = v.VehicleImages
                .OrderBy(img => img.Id)
                .Select(img => img.ImageUrl)
                .FirstOrDefault()
        }).ToList();
    }
}
