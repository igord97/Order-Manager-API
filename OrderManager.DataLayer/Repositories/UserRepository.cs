using OrderManager.DataLayer.Data;
using OrderManager.Core.Entities;
using OrderManager.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OrderManager.DataLayer.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);

        return user;
    }

    public async Task<List<User>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Users
            .OrderBy(user => user.Id)
            .Include(user => user.Orders)
            .ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<List<User>> GetByNameAsync(string name, CancellationToken ct)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .Where(u => u.Name == name)
            .ToListAsync(ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, ct);
    }

    public async Task<List<string>> GetAllNamesAsync(CancellationToken ct)
    {
        return await _context.Users
            .Select(u => u.Name)
            .ToListAsync(ct);
    }

    public async Task<User?> GetByIdWithOrdersAsync(int id, CancellationToken ct)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<List<User>> SearchUsersAsync(
    string search,
    int page,
    int pageSize,
    CancellationToken ct)
    {
        return await _context.Users
            .Where(u => u.Name.Contains(search))
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken ct)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);

        return user;
    }

    public async Task<bool> DeleteAsync(User user, CancellationToken ct)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(ct);
        
        return true;
    }
}