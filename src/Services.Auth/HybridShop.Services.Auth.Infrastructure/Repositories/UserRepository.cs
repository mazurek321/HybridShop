namespace HybridShop.Services.Auth.Infrastructure.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HybridShop.Services.Auth.Application.Dto;
using HybridShop.Services.Auth.Core.Dto;
using HybridShop.Services.Auth.Core.Interfaces;
using HybridShop.Services.Auth.Core.Models;
using HybridShop.Services.Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _dbContext;

    private static readonly Expression<Func<User, UserDto>> MapToUserDto = u => new UserDto
    {
        Id = u.Id,
        Email = u.Email,
        Name = u.Name,
        Lastname = u.Lastname,
        Gender = u.Gender.Value == UserGender.UGender.M ? 'M' : 'F',
        Role = u.Role.Value.ToString(),
        Birthday = u.Birthday
    };

    private static readonly Expression<Func<User, AdminUserDto>> MapToAdminUserDto = u => new AdminUserDto
    {
        Id = u.Id,
        Email = u.Email,
        Name = u.Name,
        Lastname = u.Lastname,
        Gender = u.Gender.Value == UserGender.UGender.M ? 'M' : 'F',
        Role = u.Role.Value.ToString(),
        Birthday = u.Birthday,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt,
        IsDeleted = u.IsDeleted,
        IsBanned = u.IsBanned,
        ActiveSessions = u.RefreshTokens
            .Select(t => new UserSessionDto
            {
                Id = t.Id,
                CreatedAt = t.CreatedAt,
                ExpiresAt = t.ExpiresAt,
                IsActive = t.ExpiresAt > DateTime.UtcNow && !t.IsRevoked
            })
    };

    public UserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserDto?> GetDtoByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .Where(u => u.Id == id)
            .Select(MapToUserDto)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<AdminUserDto?> GetAdminDtoByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .IgnoreQueryFilters()
            .Where(u => u.Id == id)
            .Select(MapToAdminUserDto)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<UserDto?> GetDtoByEmailAsync(string email)
    {
        return await _dbContext.Users
            .Where(u => u.Email == email)
            .Select(MapToUserDto)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<AdminUserDto?> GetAdminDtoByEmailAsync(string email)
    {
        return await _dbContext.Users
            .IgnoreQueryFilters()
            .Where(u => u.Email == email)
            .Select(MapToAdminUserDto)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetWithTokensByEmailAsync(string email)
    {
        return await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email);
    }


    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _dbContext.Users
            .IgnoreQueryFilters()
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
    }

    public async Task<ICollection<UserDto>> BrowseDtoUsers(int skip, int take)
    {
        return await _dbContext.Users
            .OrderBy(u => u.Id)
            .Skip(skip)
            .Take(take)
            .Select(MapToUserDto)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ICollection<AdminUserDto>> BrowseAdminDtoUsers(int skip, int take)
    {
        return await _dbContext.Users
            .IgnoreQueryFilters()
            .OrderBy(u => u.Id)
            .Skip(skip)
            .Take(take)
            .Select(MapToAdminUserDto)
            .AsNoTracking()
            .ToListAsync();
    }

    public void DeleteRefreshToken(RefreshToken token)
    {
        _dbContext.Entry(token).State = EntityState.Deleted;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}