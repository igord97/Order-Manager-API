using LearningProject1.Core.DTOs.Order;

namespace LearningProject1.Core.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllOrdersAsync(CancellationToken ct);
    Task<OrderResponseDto?> GetOrderByIdAsync(int orderId, CancellationToken ct);
    Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId, CancellationToken ct);
    Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto createOrderDto, CancellationToken ct);
}
