using LearningProject1.Core.Models;

namespace LearningProject1.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
}