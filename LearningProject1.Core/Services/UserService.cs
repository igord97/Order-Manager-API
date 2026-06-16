using LearningProject1.Core.DTOs.User;
using LearningProject1.Core.Exceptions;
using LearningProject1.Core.Interfaces;
using LearningProject1.Core.Mappers;
using LearningProject1.Core.Models;
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

    public async Task<UserResponseDto> CreateUserAsync(UserRequestDto userRequestDto, CancellationToken ct)
    {
        var normalizedEmail = userRequestDto.Email.Trim().ToLowerInvariant();
        var normalizedName = userRequestDto.Name.Trim();

        _logger.LogInformation("Trying to create user with email {Email} and name {Name}", normalizedEmail, normalizedName);

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

    public async Task<List<UserResponseDto>> GetAllUsersAsync(CancellationToken ct)
    {
        _logger.LogInformation("Getting all users");

        var users = await _userRepository.GetAllAsync(ct);
        return users.Select(UserMapper.ToResponseDto).ToList();
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        _logger.LogInformation("Getting user with id {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} was not found", userId);
            throw new NotFoundException("User not found.");
        }

        return UserMapper.ToResponseDto(user);
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

    public async Task<List<UserResponseDto>> GetByNameAsync(string userName, CancellationToken ct)
    {
        _logger.LogInformation("Trying to find user with name {UserName}", userName);

        var users = await _userRepository.GetByNameAsync(userName, ct);

        return users.Select(UserMapper.ToResponseDto).ToList();
    }

    public async Task<List<string>> GetAllNamesAsync(CancellationToken ct)
    {
        _logger.LogInformation("Trying to get all user names");
        return await _userRepository.GetAllNamesAsync(ct);
    }

    public async Task<List<UserResponseDto>> SearchUsersAsync(string search, int page, int pageSize, CancellationToken ct)
    {
        _logger.LogInformation("Searching for users...");

        if (page < 1)
        {
            _logger.LogWarning("Page was set to invalid number");
            throw new BadRequestException("Page must be greater than zero");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            _logger.LogWarning("PageSize was set to invalid number");
            throw new BadRequestException("PageSize must be between 1 and 100");
        }

        var users = await _userRepository.SearchUsersAsync(search, page, pageSize, ct);

        _logger.LogInformation("Search completed!");
        return users.Select(UserMapper.ToResponseDto).ToList();
    }

    public async Task<UpdateUserResponseDto> UpdateUserAsync(int userId, UserRequestDto userRequestDto, CancellationToken ct)
    {
        _logger.LogInformation("Trying to update user with id {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            _logger.LogWarning("Cannot update user. User with id {UserId} was not found", userId);
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

    public async Task<bool> DeleteUserAsync(int userId, CancellationToken ct)
    {
        _logger.LogInformation("Trying to delete user with id {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            _logger.LogWarning("Cannot delete user. User with id {UserId} was not found", userId);
            return false;
        }

        await _userRepository.DeleteAsync(user, ct);
        _logger.LogInformation("User with id {UserId} deleted successfully", userId);
        return true;
    }
}