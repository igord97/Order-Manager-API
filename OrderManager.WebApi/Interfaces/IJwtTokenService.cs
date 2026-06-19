using OrderManager.DataLayer.BusinessObjects.Persistent.dbo;

namespace OrderManager.WebApi.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
}