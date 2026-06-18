using LearningProject1.Core.DTOs.Order;
using LearningProject1.Core.Commands;

namespace LearningProject1.Core.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllOrdersAsync(CancellationToken ct);
    Task<OrderResponseDto> GetOrderByIdAsync(int orderId, CancellationToken ct);
    Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId, CancellationToken ct);
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderCommand createOrderCommand, CancellationToken ct);
    Task<OrderResponseDto> UpdateOrderAsync(int orderId, UpdateOrderRequestDto orderRequestDto, CancellationToken ct);
    Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct);
}
