using FSM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FSM.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<Technician> Technicians { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
}