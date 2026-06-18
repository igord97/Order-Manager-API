using OrderManager.DataLayer.Data;
using OrderManager.Core.Entities;
using OrderManager.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OrderManager.DataLayer.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> AddAsync(Order order, CancellationToken ct)
    {
        await _context.Orders.AddAsync(order, ct);
        await _context.SaveChangesAsync(ct);

        return order;
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Orders.ToListAsync(ct);
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<Order> UpdateAsync(Order order, CancellationToken ct)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(ct);

        return order;
    }

    public async Task<bool> DeleteAsync(Order order, CancellationToken ct)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}