using LearningProject1.Core.DTOs.User;
using LearningProject1.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace LearningProject1.WebApi.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<UserResponseDto>> GetAll(CancellationToken ct)
    {
        var users = await _userService.GetAllUsersAsync(ct);
        
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int userId, CancellationToken ct)
    {
        var user = await _userService.GetUserByIdAsync(userId, ct);
        if (user == null)
           return NotFound();

        return Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserResponseDto>> GetByEmail(string email, CancellationToken ct)
    {
        var user = await _userService.GetByEmailAsync(email, ct);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<List<UserResponseDto>>> GetByName(string userName, CancellationToken ct)
    {
        var users = await _userService.GetByNameAsync(userName, ct);
        return Ok(users);
    }

    [HttpGet("names")]
    public async Task<ActionResult<List<string>>> GetAllNames(CancellationToken ct)
    {
        var names = await _userService.GetAllNamesAsync(ct);
        return Ok(names);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<UserResponseDto>>> SearchUsers(
        [FromQuery] string search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var users = await _userService.SearchUsersAsync(search, page, pageSize, ct);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] UserRequestDto userRequestDto, CancellationToken ct)
    {
        var createdUser = await _userService.CreateUserAsync(userRequestDto, ct);

        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(int userId, [FromBody] UserRequestDto userRequestDto, CancellationToken ct)
    {
        var updatedUser = await _userService.UpdateUserAsync(userId, userRequestDto, ct);
        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int userId, CancellationToken ct)
    {
        await _userService.DeleteUserAsync(userId, ct);
        return NoContent();
    }
}
