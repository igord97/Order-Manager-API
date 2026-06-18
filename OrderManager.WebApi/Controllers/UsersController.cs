using Azure.Core;
using OrderManager.Core.DTOs.User;
using OrderManager.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OrderManager.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // -------------------------------------------------------------------
    // USER ENDPOINTS
    // -------------------------------------------------------------------

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetMe(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(currentUserId, ct);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UpdateUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateUserResponseDto>> UpdateMe(
        [FromBody] UpdateMyProfileRequestDto updateMyProfileRequestDto,
        CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();

        var existingUser = await _userService.GetUserByIdAsync(currentUserId, ct);

        if (existingUser == null)
            return NotFound();

        var oldPasswordProvided = !string.IsNullOrWhiteSpace(updateMyProfileRequestDto.OldPassword);
        var newPasswordProvided = !string.IsNullOrWhiteSpace(updateMyProfileRequestDto.NewPassword);

        if (oldPasswordProvided != newPasswordProvided)
            return BadRequest("Both old password and new password must be provided to change password.");

        var updatedUser = await _userService.UpdateMyProfileAsync(currentUserId, updateMyProfileRequestDto, ct);

        if (updatedUser == null)
            return BadRequest("Old password is incorrect.");

        return Ok(updatedUser);
    }

    // -------------------------------------------------------------------
    // ADMIN ENDPOINTS
    // -------------------------------------------------------------------

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> AdminGetAllUsers(CancellationToken ct)
    {
        var users = await _userService.GetAllUsersAsync(ct);

        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{userId:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> AdminGetUserById(
        int userId,
        CancellationToken ct)
    {
        var user = await _userService.GetUserByIdAsync(userId, ct);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/search")]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> AdminSearchUsers(
        [FromQuery] string search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(search))
            return BadRequest("Search value is required.");

        if (page < 1 || pageSize < 1)
            return BadRequest("Page and pageSize must be greater than zero.");

        var users = await _userService.SearchUsersAsync(search, page, pageSize, ct);

        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/{userId:int}")]
    [ProducesResponseType(typeof(UpdateUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateUserResponseDto>> AdminUpdateUser(
        int userId,
        [FromBody] UserRequestDto userRequestDto,
        CancellationToken ct)
    {
        var existingUser = await _userService.GetUserByIdAsync(userId, ct);

        if (existingUser == null)
            return NotFound();

        var updatedUser = await _userService.UpdateUserAsync(userId, userRequestDto, ct);

        return Ok(updatedUser);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminDeleteUser(
        int userId,
        CancellationToken ct)
    {
        var deleted = await _userService.DeleteUserAsync(userId, ct); // would be better to have property IsDeactivated, but can leave it like this for now (hard delete)

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // =========================
    // HELPER
    // =========================

    private bool TryGetCurrentUserId(out int userId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out userId);
    }
}