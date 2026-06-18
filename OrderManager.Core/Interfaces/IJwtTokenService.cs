using OrderManager.Core.Models;

namespace OrderManager.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
}