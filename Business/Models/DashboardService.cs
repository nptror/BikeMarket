using Business.Interface;
using DataAccess.Interface;
using DTO.Dashboard;

namespace Business.Service;

public class DashboardService : IDashboardService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;

    public DashboardService(
        IVehicleRepository vehicleRepository,
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository)
    {
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<AdminDashboardDTO> GetAdminDashboardStatsAsync()
    {
        var allVehicles = await _vehicleRepository.GetAllWithIncludesAsync();
        var allUsers = await _userRepository.GetAllAsync();
        var allOrders = await _orderRepository.GetAllAsync();
        var allBrands = await _brandRepository.GetAllAsync();
        var allCategories = await _categoryRepository.GetAllAsync();

        var now = DateTime.Now;
        var weekAgo = now.AddDays(-7);
        var monthStart = new DateTime(now.Year, now.Month, 1);

        return new AdminDashboardDTO
        {
            PendingPostsCount = allVehicles.Count(v => 
                v.Status != null && 
                (v.Status.Equals("pending", StringComparison.OrdinalIgnoreCase))),
            
            TotalVehiclesCount = allVehicles.Count,
            
            VehiclesThisWeek = allVehicles.Count(v => 
                v.CreatedAt.HasValue && v.CreatedAt.Value >= weekAgo),
            
            TotalUsersCount = allUsers.Count,
            
            UsersThisMonth = allUsers.Count(u => 
                u.CreatedAt.HasValue && u.CreatedAt.Value >= monthStart),
            
            OrdersThisMonth = allOrders.Count(o => 
                o.CreatedAt.HasValue && o.CreatedAt.Value >= monthStart),
            
            TotalBrandsCount = allBrands.Count,
            
            TotalCategoriesCount = allCategories.Count
        };
    }
}
