using LearningProject1.Core.Commands;
using LearningProject1.Core.DTOs.Order;
using LearningProject1.Core.Exceptions;
using LearningProject1.Core.Interfaces;
using LearningProject1.Core.Mappers;
using Microsoft.Extensions.Logging;

namespace LearningProject1.Core.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderCommand createOrderCommand, CancellationToken ct)
    {
        _logger.LogInformation("Creating order for user {UserId}", createOrderCommand.UserId);

        if (string.IsNullOrWhiteSpace(createOrderCommand.Product))
        {
            _logger.LogWarning("Product is required.");
            throw new BadRequestException("Product is required.");
        }

        if (createOrderCommand.Quantity <= 0)
        {
            _logger.LogWarning(
                "Order creation failed because quantity was invalid: {Quantity}",
                createOrderCommand.Quantity);

            throw new BadRequestException("Quantity must be greater than zero.");
        }

        if (createOrderCommand.Price <= 0)
        {
            _logger.LogWarning(
                "Order creation failed because price was invalid: {Price}",
                createOrderCommand.Price);

            throw new BadRequestException("Price must be greater than zero.");
        }

        var user = await _userRepository.GetByIdAsync(createOrderCommand.UserId, ct);

        if (user is null)
        {
            _logger.LogWarning("User with id {UserId} was not found", createOrderCommand.UserId);
            throw new BadRequestException("UserId must belong to an existing user.");
        }

        var order = OrderMapper.ToEntity(createOrderCommand);

        order.Total = createOrderCommand.Quantity * createOrderCommand.Price;

        var createdOrder = await _orderRepository.AddAsync(order, ct);

        _logger.LogInformation(
            "Order created successfully with id {OrderId} for user {UserId}",
            createdOrder.Id,
            createdOrder.UserId);

        return OrderMapper.ToResponseDto(createdOrder);
    }

    public async Task<List<OrderResponseDto>> GetAllOrdersAsync(CancellationToken ct)
    {
        _logger.LogInformation("Getting all orders");

        var orders = await _orderRepository.GetAllAsync(ct);
        return orders.Select(OrderMapper.ToResponseDto).ToList();
    }

    public async Task<OrderResponseDto> GetOrderByIdAsync(int orderId, CancellationToken ct)
    {
        _logger.LogInformation("Getting order with id {OrderId}", orderId);

        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order is null)
        {
            _logger.LogWarning("Order with {OrderId} was not found", orderId);
            throw new NotFoundException("Order with this ID not found");
        }

        var responseOrder = OrderMapper.ToResponseDto(order);
        return responseOrder;
    }

    public async Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId, CancellationToken ct)
    {
        _logger.LogInformation("Getting orders from user with id {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
        {
            _logger.LogWarning("User with id {UserId} was not found", userId);
            throw new NotFoundException("User with this ID not found");
        }

        var orders = await _orderRepository.GetByUserIdAsync(userId, ct);
        return orders.Select(OrderMapper.ToResponseDto).ToList();
    }

    public async Task<OrderResponseDto> UpdateOrderAsync(int orderId, UpdateOrderRequestDto updateOrderRequest, CancellationToken ct)
    {
        _logger.LogInformation("Trying to update order with id {OrderId}", orderId);

        var existingOrder = await _orderRepository.GetByIdAsync(orderId, ct);

        if (existingOrder is null)
        {
            _logger.LogWarning("Order with id {OrderId} was not found", orderId);
            throw new NotFoundException("Order with this ID not found");
        }

        if (string.IsNullOrWhiteSpace(updateOrderRequest.Product))
        {
            _logger.LogWarning("Order update failed because product was empty.");
            throw new BadRequestException("Product is required.");
        }

        if (updateOrderRequest.Price <= 0)
        {
            _logger.LogWarning(
                "Order update failed because price was invalid: {Price}",
                updateOrderRequest.Price);

            throw new BadRequestException("Price must be greater than zero.");
        }

        existingOrder.Product = updateOrderRequest.Product.Trim();
        existingOrder.Price = updateOrderRequest.Price;
        existingOrder.Quantity = updateOrderRequest.Quantity;
        existingOrder.Total = updateOrderRequest.Quantity * updateOrderRequest.Price;

        var updatedOrder = await _orderRepository.UpdateAsync(existingOrder, ct);

        _logger.LogInformation("Order with id {OrderId} updated successfully", orderId);
        return OrderMapper.ToResponseDto(updatedOrder);
    }

    public async Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct)
    {
        _logger.LogInformation("Trying to delete order with id {OrderId}", orderId);

        var existingOrder = await _orderRepository.GetByIdAsync(orderId, ct);

        if (existingOrder is null)
        {
            _logger.LogWarning("Order with id {OrderId} was not found", orderId);
            return false;
        }

        await _orderRepository.DeleteAsync(existingOrder, ct);

        _logger.LogInformation("Order with id {OrderId} deleted successfully", orderId);
        return true;
    }
}