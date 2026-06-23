using OrderManager.WebApi.Models.User;
using OrderManager.WebApi.Interfaces;
using OrderManager.WebApi.Mappers;
using OrderManager.DataLayer.BusinessObjects.Persistent.dbo;
using OrderManager.DataLayer.Interfaces;
using OrderManager.Core.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace OrderManager.WebApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly ILogger<UserService> _logger;


    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = new PasswordHasher<User>();
        _logger = logger;
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

    public async Task<UpdateUserResponseDto?> UpdateMyProfileAsync(
    int userId,
    UpdateMyProfileRequestDto request,
    CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user == null)
            return null;

        user.Name = request.Name;
        user.Email = request.Email;

        var oldPasswordProvided = !string.IsNullOrWhiteSpace(request.OldPassword);
        var newPasswordProvided = !string.IsNullOrWhiteSpace(request.NewPassword);

        if (oldPasswordProvided || newPasswordProvided)
        {
            if (!oldPasswordProvided || !newPasswordProvided)
                throw new ArgumentException("Both old password and new password must be provided.");

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.OldPassword!);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return null;

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword!);
        }

        var updatedUser = await _userRepository.UpdateAsync(user, ct);

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