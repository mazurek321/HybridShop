using HybridShop.Services.Order.Core.Models.Outbox;
using Microsoft.EntityFrameworkCore;
using OrderAggregate = HybridShop.Services.Order.Core.Models.Order.Order;

namespace HybridShop.Services.Order.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<OrderAggregate> Orders => Set<OrderAggregate>();
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderAggregate>(builder =>
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.OwnsMany(o => o.Items, item =>
            {
                item.ToTable("OrderItems");
                item.WithOwner().HasForeignKey("OrderId");
                item.Property<Guid>("Id");
                item.HasKey("Id");
                item.Property(i => i.ProductId).IsRequired();
                item.Property(i => i.Title).HasMaxLength(255);
                item.Property(i => i.Price).HasColumnType("decimal(18,2)");
            });
        });
    }
}