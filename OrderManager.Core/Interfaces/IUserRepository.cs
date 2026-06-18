using OrderManager.Core.Models;

namespace OrderManager.Core.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task<User> AddAsync(User user, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<List<User>> GetByNameAsync(string name, CancellationToken ct);
    Task<List<string>> GetAllNamesAsync(CancellationToken ct);
    Task<List<User>> SearchUsersAsync(
    string search,
    int page,
    int pageSize,
    CancellationToken ct);
    Task<User> UpdateAsync(User user, CancellationToken ct);
    Task<bool> DeleteAsync(User user, CancellationToken ct);
    Task<User?> GetByIdWithOrdersAsync(int id, CancellationToken ct);
}
