using LearningProject1.Core.DTOs.Order;
using LearningProject1.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LearningProject1.WebApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<OrderResponseDto>> GetAll(CancellationToken ct)
    {
        var orders = await _orderService.GetAllOrdersAsync(ct);
        return Ok(orders);
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int orderId, CancellationToken ct)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<OrderResponseDto>> GetByUserId(int userId, CancellationToken ct)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId, ct);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] OrderRequestDto orderRequestDto, CancellationToken ct)
    {
        var createdOrder = await _orderService.CreateOrderAsync(orderRequestDto, ct);

        return CreatedAtAction(nameof(GetById), new { orderId = createdOrder.Id}, createdOrder);
    }
}