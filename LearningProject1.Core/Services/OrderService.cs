using LearningProject1.Core.Interfaces;
using LearningProject1.Core.DTOs.Order;
using LearningProject1.Core.Exceptions;
using LearningProject1.Core.Mappers;
using Microsoft.Extensions.Logging;

namespace LearningProject1.Core.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, ILogger<UserService> logger)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<OrderResponseDto>> GetAllOrdersAsync(CancellationToken ct)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        return orders.Select(OrderMapper.ToResponseDto).ToList();
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(int orderId, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order is null)
        {
            _logger.LogError("Order with {OrderId} was not found", orderId);
            throw new NotFoundException("Order with this ID not found");
        }

        var responseOrder = OrderMapper.ToResponseDto(order);
        return responseOrder;
    }

    public async Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
        {
            _logger.LogWarning("User with id {UserId} was not found", userId);
            throw new NotFoundException("User with this ID not found");
        }

        var orders = await _orderRepository.GetByUserIdAsync(userId, ct);
        return orders.Select(OrderMapper.ToResponseDto).ToList();
    }

    public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderRequestDto, CancellationToken ct)
    {


        if (string.IsNullOrWhiteSpace(orderRequestDto.Product))
        {
            _logger.LogWarning("Product is required.");
            throw new BadRequestException("Product is required.");
        }

        var user = await _userRepository.GetByIdAsync(orderRequestDto.UserId, ct);

        if (user is null)
        {
            _logger.LogWarning("User with id {UserId} was not found", orderRequestDto.UserId);
            throw new BadRequestException("UserId must belong to an existing user.");
        }

        var order = OrderMapper.ToEntity(orderRequestDto);

        var createdOrder = await _orderRepository.AddAsync(order, ct);

        _logger.LogInformation("Order created successfully with id {OrderId} for user {UserId}", createdOrder.Id, createdOrder.UserId);

        return OrderMapper.ToResponseDto(createdOrder);
    }
}