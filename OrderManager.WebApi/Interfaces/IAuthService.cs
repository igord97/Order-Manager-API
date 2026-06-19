using OrderManager.WebApi.Models.Auth;

namespace OrderManager.WebApi.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
}