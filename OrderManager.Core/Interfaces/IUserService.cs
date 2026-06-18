using OrderManager.Core.DTOs.User;

namespace OrderManager.Core.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUsersAsync(CancellationToken ct);
    Task<UserResponseDto> GetUserByIdAsync(int id, CancellationToken ct);
    Task<UserResponseDto> CreateUserAsync(UserRequestDto user, CancellationToken ct);
    Task<UserResponseDto> GetByEmailAsync(string email, CancellationToken ct);
    Task<List<UserResponseDto>> GetByNameAsync(string name, CancellationToken ct);
    Task<List<string>> GetAllNamesAsync(CancellationToken ct);
    Task<List<UserResponseDto>> SearchUsersAsync(string search, int page, int pageSize, CancellationToken ct);
    Task<UpdateUserResponseDto> UpdateUserAsync(int id, UserRequestDto updateUserDto, CancellationToken ct);
    Task<UpdateUserResponseDto> UpdateMyProfileAsync(int id, UpdateMyProfileRequestDto updateMyProfileRequestDto, CancellationToken ct);
    Task<bool> DeleteUserAsync(int id, CancellationToken ct);
}
    