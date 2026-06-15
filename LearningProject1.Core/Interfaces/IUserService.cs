using LearningProject1.Core.DTOs.User;

namespace LearningProject1.Core.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUsersAsync(CancellationToken ct);
    Task<UserResponseDto?> GetUserByIdAsync(int id, CancellationToken ct);
    Task<UserResponseDto> CreateUserAsync(UserRequestDto user, CancellationToken ct);
    Task<UserResponseDto?> GetByEmailAsync(string email, CancellationToken ct);
    Task<List<UserResponseDto>> GetByNameAsync(string name, CancellationToken ct);
    Task<List<string>> GetAllNamesAsync(CancellationToken ct);
    Task<List<UserResponseDto>> SearchUsersAsync(string search, int page, int pageSize, CancellationToken ct);
    Task<UpdateUserResponseDto> UpdateUserAsync(int id, UserRequestDto updateUserDto, CancellationToken ct);
    Task DeleteUserAsync(int id, CancellationToken ct);
}
    