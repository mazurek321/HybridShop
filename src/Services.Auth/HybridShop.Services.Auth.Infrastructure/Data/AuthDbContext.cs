using HybridShop.Services.Auth.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HybridShop.Services.Auth.Infrastructure.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
                
            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Name)
                .HasMaxLength(100);

            entity.Property(u => u.Lastname)
                .HasMaxLength(100);

            entity.Property(u => u.Gender)
                .HasConversion(
                    g => g.Value.ToString(),
                    s => new UserGender((UserGender.UGender)Enum.Parse(typeof(UserGender.UGender), s)) 
                )
                .HasColumnName("Gender")
                .HasMaxLength(1);

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion(
                    role => role.Value.ToString(), 
                    value => new UserRole((URole)Enum.Parse(typeof(URole), value)) 
                )
                .HasColumnName("Role");

            entity.Property(u => u.Birthday)
                .IsRequired();

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.Property(u => u.UpdatedAt)
                .IsRequired();

            entity.HasMany(u => u.RefreshTokens)
                .WithOne()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(u => u.RefreshTokens)
                .HasField("_refreshTokens")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.Property(u => u.IsDeleted)
                .HasColumnName("IsDeleted")
                .IsRequired();
            
            entity.Property(u => u.IsBanned)
                .IsRequired();


            entity.HasQueryFilter(u => !u.IsDeleted && !u.IsBanned);
        });




        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .ValueGeneratedNever();

            entity.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasIndex(t => t.Token)
                .IsUnique();

            entity.Property(t => t.ExpiresAt)
                .IsRequired();

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.Property(t => t.RevokedAt)
                .IsRequired(false);
        });
    }
}