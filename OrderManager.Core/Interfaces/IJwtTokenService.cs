using OrderManager.Core.Entities;

namespace OrderManager.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
}