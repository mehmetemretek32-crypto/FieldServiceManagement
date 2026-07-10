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
    public DbSet<Customer> Customers { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }

    // 🔥 YENİ EKLENEN ARA TABLOLAR (Zimmet ve Kullanılan Malzemeler)
    public DbSet<TechnicianInventory> TechnicianInventories { get; set; }
    public DbSet<WorkOrderInventory> WorkOrderInventories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔥 PRO DOKUNUŞ: Zincirleme silme (Cascade Delete) hatalarını önlemek için kurallar

        // Teknisyen silinirse zimmet tablosu etkilenmesin (hata versin veya manuel yönetilsin)
        modelBuilder.Entity<TechnicianInventory>()
            .HasOne(ti => ti.Technician)
            .WithMany(t => t.InventoryItems)
            .HasForeignKey(ti => ti.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TechnicianInventory>()
            .HasOne(ti => ti.InventoryItem)
            .WithMany()
            .HasForeignKey(ti => ti.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // İş Emri silinirse kullanılan malzeme kayıtları otomatik uçmasın (geçmiş tutmak için)
        modelBuilder.Entity<WorkOrderInventory>()
            .HasOne(wi => wi.WorkOrder)
            .WithMany(w => w.UsedParts)
            .HasForeignKey(wi => wi.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkOrderInventory>()
            .HasOne(wi => wi.InventoryItem)
            .WithMany()
            .HasForeignKey(wi => wi.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}