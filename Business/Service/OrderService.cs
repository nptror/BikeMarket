using Business.Interface;
using DataAccess.Interface;
using DataAccess.Models;

namespace Business.Service;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IVehicleRepository vehicleRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
    }

    public Task<List<Order>> GetAllAsync()
    {
        return _orderRepository.GetAllWithIncludesAsync();
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        return _orderRepository.GetByIdWithIncludesAsync(id);
    }

    public Task CreateAsync(Order order)
    {
        return _orderRepository.AddAsync(order);
    }

    public Task UpdateAsync(Order order)
    {
        return _orderRepository.UpdateAsync(order);
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order != null)
        {
            await _orderRepository.DeleteAsync(order);
        }
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _orderRepository.ExistsAsync(id);
    }

    public async Task<Order?> BuyNowAsync(int buyerId, int vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null)
        {
            return null;
        }

        var order = new Order
        {
            BuyerId = buyerId,
            SellerId = vehicle.SellerId,
            VehicleId = vehicle.Id,
            TotalAmount = vehicle.Price,
            Status = "pending",
            PaymentStatus = "unpaid",
            CreatedAt = DateTime.Now
        };

        await _orderRepository.AddAsync(order);
        return order;
    }

    public async Task<bool> PayAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return false;
        }

        order.PaymentStatus = "paid";
        order.Status = "paid";
        order.UpdatedAt = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public Task<List<User>> GetUsersAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public Task<List<Vehicle>> GetVehiclesAsync()
    {
        return _vehicleRepository.GetAllWithIncludesAsync();
    }
}
