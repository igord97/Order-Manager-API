using OrderManager.WebApi.Models.Auth;
using OrderManager.WebApi.Interfaces;
using OrderManager.DataLayer.BusinessObjects.Persistent.dbo;
using OrderManager.DataLayer.Interfaces;
using OrderManager.Core.Constants;
using Microsoft.AspNetCore.Identity;

namespace OrderManager.WebApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, ct);

        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = RoleConstants.User,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, ct);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "1440"))
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var accessToken = _jwtTokenService.GenerateAccessToken(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "1440"))
        };
    }
}