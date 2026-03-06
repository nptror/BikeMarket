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

    public Task<Order?> GetLatestPaidOrderAsync(int buyerId, int sellerId)
    {
        return _orderRepository.GetLatestPaidOrderAsync(buyerId, sellerId);
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
        if (vehicle == null || vehicle.Status == "sold" || vehicle.Status == "paid")
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

        var vehicle = await _vehicleRepository.GetByIdAsync(order.VehicleId);
        if (vehicle != null)
        {
            vehicle.Status = "paid";
            await _vehicleRepository.UpdateAsync(vehicle);
        }

        return true;
    }

    public Task<List<Order>> GetPaidOrdersByBuyerAsync(int buyerId)
    {
        return _orderRepository.GetPaidOrdersByBuyerAsync(buyerId);
    }

    public Task<List<Order>> GetPaidOrdersBySellerAsync(int sellerId)
    {
        return _orderRepository.GetPaidOrdersBySellerAsync(sellerId);
    }

    public async Task ConfirmHandoverAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) return;

        order.Status = "shipped";
        order.UpdatedAt = DateTime.Now;
        await _orderRepository.UpdateAsync(order);
    }

    public async Task ConfirmReceivedAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) return;

        order.Status = "completed";
        order.UpdatedAt = DateTime.Now;
        await _orderRepository.UpdateAsync(order);

        var vehicle = await _vehicleRepository.GetByIdAsync(order.VehicleId);
        if (vehicle != null)
        {
            vehicle.Status = "sold";
            await _vehicleRepository.UpdateAsync(vehicle);
        }
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
