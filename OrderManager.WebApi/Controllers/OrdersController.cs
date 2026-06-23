using OrderManager.WebApi.Models.Order;
using OrderManager.WebApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderManager.WebApi.Controllers;

[Authorize]
public class OrdersController : BaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(
        IOrderService orderService)
    {
        _orderService = orderService;
    }

    // -------------------------------------------------------------------
    // USER ENDPOINTS
    // -------------------------------------------------------------------

    [HttpPost("my")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderResponseDto>> CreateMyOrder(
        [FromBody] CreateMyOrderRequestDto createMyOrderRequestDto,
        CancellationToken ct)
    {
        var command = new CreateOrderCommandDto
        {
            UserId = CurrentUserId,
            Product = createMyOrderRequestDto.Product,
            Quantity = createMyOrderRequestDto.Quantity,
            Price = createMyOrderRequestDto.Price
        };

        var createdOrder = await _orderService.CreateOrderAsync(command, ct);

        return CreatedAtAction(
            nameof(GetMyOrderById),
            new { orderId = createdOrder.Id },
            createdOrder);
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders(CancellationToken ct)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(CurrentUserId, ct);

        return Ok(orders);
    }

    [HttpGet("my/{orderId:int}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetMyOrderById(
        int orderId,
        CancellationToken ct)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (order == null)
            return NotFound();

        if (order.UserId != CurrentUserId)
            return Forbid();

        return Ok(order);
    }

    [HttpPut("my/{orderId:int}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> UpdateMyOrder(
        int orderId,
        [FromBody] UpdateOrderRequestDto orderRequestDto,
        CancellationToken ct)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (existingOrder == null)
            return NotFound();

        if (existingOrder.UserId != CurrentUserId)
            return Forbid();

        var updatedOrder = await _orderService.UpdateOrderAsync(orderId, orderRequestDto, ct);

        return Ok(updatedOrder);
    }

    [HttpDelete("my/{orderId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMyOrder(
        int orderId,
        CancellationToken ct)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (existingOrder == null)
            return NotFound();

        if (existingOrder.UserId != CurrentUserId)
            return Forbid();

        var deleted = await _orderService.DeleteOrderAsync(orderId, ct);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // -------------------------------------------------------------------
    // ADMIN ENDPOINTS
    // -------------------------------------------------------------------

    [Authorize(Roles = "Admin")]
    [HttpPost("admin")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrderResponseDto>> AdminCreateOrder(
        [FromBody] CreateOrderForUserRequestDto createOrderForUserDto,
        CancellationToken ct)
    {
        var command = new CreateOrderCommandDto
        {
            UserId = createOrderForUserDto.UserId,
            Product = createOrderForUserDto.Product,
            Quantity = createOrderForUserDto.Quantity,
            Price = createOrderForUserDto.Price
        };

        var createdOrder = await _orderService.CreateOrderAsync(command, ct);

        return CreatedAtAction(
            nameof(AdminGetOrderById),
            new { orderId = createdOrder.Id },
            createdOrder);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> AdminGetAllOrders(CancellationToken ct)
    {
        var orders = await _orderService.GetAllOrdersAsync(ct);

        return Ok(orders);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{orderId:int}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> AdminGetOrderById(
        int orderId,
        CancellationToken ct)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> AdminGetOrdersByUserId(
        int userId,
        CancellationToken ct)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId, ct);

        return Ok(orders);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/{orderId:int}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> AdminUpdateOrder(
        int orderId,
        [FromBody] UpdateOrderRequestDto orderRequestDto,
        CancellationToken ct)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (existingOrder == null)
            return NotFound();

        var updatedOrder = await _orderService.UpdateOrderAsync(orderId, orderRequestDto, ct);

        return Ok(updatedOrder);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{orderId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminDeleteOrder(
        int orderId,
        CancellationToken ct)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(orderId, ct);

        if (existingOrder == null)
            return NotFound();

        var deleted = await _orderService.DeleteOrderAsync(orderId, ct);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}