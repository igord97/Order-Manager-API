using Microsoft.EntityFrameworkCore;
using LearningProject1.Core.Models;

namespace LearningProject1.DataLayer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
}
