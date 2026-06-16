using LearningProject1.Core.DTOs.Order;
using LearningProject1.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningProject1.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] OrderRequestDto orderRequestDto, CancellationToken ct)
    {
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            orderRequestDto.UserId = userId;
        }

        var createdOrder = await _orderService.CreateOrderAsync(orderRequestDto, ct); // would be better to have DTO for specific user

        return CreatedAtAction(nameof(GetById), new { orderId = createdOrder.Id }, createdOrder);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll(CancellationToken ct)
    {
        var orders = await _orderService.GetAllOrdersAsync(ct);
        return Ok(orders);
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        var orders = await _orderService.GetOrdersByUserIdAsync(userId, ct);

        return Ok(orders);
    }

    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetById(int orderId, CancellationToken ct)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (order == null)
            return NotFound();

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            if (order.UserId != userId)
                return Forbid();
        }

        return Ok(order);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetByUserId(int userId, CancellationToken ct)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId, ct);
        return Ok(orders);
    }

    [HttpPut("{orderId}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> UpdateOrder(int orderId, [FromBody] UpdateOrderRequestDto orderRequestDto, CancellationToken ct)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (existingOrder == null)
            return NotFound();

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            if (existingOrder.UserId != userId)
                return Forbid();
        }

        var updatedOrder = await _orderService.UpdateOrderAsync(orderId, orderRequestDto, ct);

        return Ok(updatedOrder);
    }

    [HttpDelete("{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(int orderId, CancellationToken ct)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (existingOrder == null)
            return NotFound();

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            if (existingOrder.UserId != userId)
                return Forbid();
        }

        var deleted = await _orderService.DeleteOrderAsync(orderId, ct);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // HELPER

    private bool TryGetCurrentUserId(out int userId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out userId);
    }
}