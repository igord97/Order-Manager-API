using LearningProject1.Core.DTOs.User;
using LearningProject1.Core.Exceptions;
using LearningProject1.Core.Mappers;
using LearningProject1.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace LearningProject1.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync(CancellationToken ct)
    {
        var users = await _userRepository.GetAllAsync(ct);
        return users.Select(UserMapper.ToResponseDto).ToList();
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int id, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} was not found", id);
            throw new NotFoundException("User not found");
        }

        return UserMapper.ToResponseDto(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(UserRequestDto userRequestDto, CancellationToken ct)
    {
        var normalizedEmail = userRequestDto.Email.Trim().ToLowerInvariant();
        var normalizedName = userRequestDto.Name.Trim();

        _logger.LogInformation("Trying to create user with email {Email}", userRequestDto.Email);

        if (await _userRepository.EmailExistsAsync(normalizedEmail, ct))
        {
            _logger.LogWarning("User with email {Email} already exists", normalizedEmail);
            throw new ConflictException("Email already exists");
        }

        var user = UserMapper.ToEntity(userRequestDto);
        user.Email = normalizedEmail;
        user.Name = normalizedName;

        var createdUser = await _userRepository.AddAsync(user, ct);

        _logger.LogInformation(
            "User created successfully with id {UserId} and email {Email}",
            createdUser.Id,
            createdUser.Email
        );

        return UserMapper.ToResponseDto(createdUser);
    }

    public async Task<UpdateUserResponseDto> UpdateUserAsync(int id, UserRequestDto userRequestDto, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            _logger.LogWarning("Cannot update user. User with id {UserId} was not found", id);
            throw new NotFoundException("User not found");
        }

        var normalizedEmail = userRequestDto.Email.Trim().ToLowerInvariant();

        if (user.Email != normalizedEmail && await _userRepository.EmailExistsAsync(normalizedEmail, ct))
        {
            _logger.LogWarning("Cannot update user. Email {Email} already exists", normalizedEmail);
            throw new ConflictException("Email already exists");
        }

        user.Name = userRequestDto.Name.Trim();
        user.Email = normalizedEmail;

        var updatedUser = await _userRepository.UpdateAsync(user, ct);

        _logger.LogInformation("User with id {UserId} updated successfully", updatedUser.Id);

        return UserMapper.ToUpdateResponseDto(updatedUser);
    }

    public async Task DeleteUserAsync(int id, CancellationToken ct)
    {
        _logger.LogInformation("Trying to delete user with id {UserId}", id);

        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            _logger.LogWarning("Cannot delete user. User with id {UserId} was not found", id);
            throw new NotFoundException("User not found");
        }

        await _userRepository.DeleteAsync(user, ct);
        _logger.LogInformation("User with id {UserId} deleted successfully", id);
    }

    public async Task<UserResponseDto> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(email, ct);
        if (user is null)
        {
            _logger.LogWarning("User with email {Email} was not found", email);
            throw new NotFoundException("User not found");
        }

        return UserMapper.ToResponseDto(user);
    }

    public async Task<List<UserResponseDto>> GetByNameAsync(string name, CancellationToken ct)
    {
        var users = await _userRepository.GetByNameAsync(name, ct);

        return users.Select(UserMapper.ToResponseDto).ToList();
    }

    public async Task<List<string>> GetAllNamesAsync(CancellationToken ct)
    {
        return await _userRepository.GetAllNamesAsync(ct);
    }

    public async Task<List<UserResponseDto>> SearchUsersAsync(string search, int page, int pageSize, CancellationToken ct)
    {
        if (page < 1)
        {
            throw new BadRequestException("Page must be greater than zero.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new BadRequestException("PageSize must be between 1 and 100.");
        }

        var users = await _userRepository.SearchUsersAsync(search, page, pageSize, ct);
    
        return users.Select(UserMapper.ToResponseDto).ToList();
    }
}