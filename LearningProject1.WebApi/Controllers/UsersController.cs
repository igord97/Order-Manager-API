using LearningProject1.Core.DTOs.User;
using LearningProject1.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningProject1.WebApi.Controllers;

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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll(CancellationToken ct)
    {
        var users = await _userService.GetAllUsersAsync(ct);

        return Ok(users);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetById(int userId, CancellationToken ct)
    {
        var user = await _userService.GetUserByIdAsync(userId, ct);

        if (user == null)
            return NotFound();

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized();

            if (userId != currentUserId)
                return Forbid();
        }

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("email/{userEmail}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetByEmail(string userEmail, CancellationToken ct)
    {
        var user = await _userService.GetByEmailAsync(userEmail, ct);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("name/{userName}")]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetByName(string userName, CancellationToken ct)
    {
        var users = await _userService.GetByNameAsync(userName, ct);

        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("names")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<string>>> GetAllNames(CancellationToken ct)
    {
        var names = await _userService.GetAllNamesAsync(ct);

        return Ok(names);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> SearchUsers(
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

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] UserRequestDto userRequestDto, CancellationToken ct)
    {
        var createdUser = await _userService.CreateUserAsync(userRequestDto, ct);

        return CreatedAtAction(nameof(GetById), new { userId = createdUser.Id }, createdUser);
    }

    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(UpdateUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateUserResponseDto>> UpdateUser(
        int userId,
        [FromBody] UserRequestDto userRequestDto,
        CancellationToken ct)
    {
        var existingUser = await _userService.GetUserByIdAsync(userId, ct);

        if (existingUser == null)
            return NotFound();

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized();

            if (userId != currentUserId)
                return Forbid();
        }

        var updatedUser = await _userService.UpdateUserAsync(userId, userRequestDto, ct);

        return Ok(updatedUser);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int userId, CancellationToken ct)
    {
        var deleted = await _userService.DeleteUserAsync(userId, ct);

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