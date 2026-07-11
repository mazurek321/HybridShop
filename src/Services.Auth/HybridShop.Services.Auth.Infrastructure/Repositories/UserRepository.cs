namespace HybridShop.Services.Auth.Infrastructure.Repositories;

using HybridShop.Services.Auth.Core.Dto;
using HybridShop.Services.Auth.Core.Interfaces;
using HybridShop.Services.Auth.Core.Models;
using HybridShop.Services.Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _dbContext;

    public UserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserDto?> GetDtoByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .Where(u => u.Id == id)
            .Select(u=> new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Lastname = u.Lastname,
                Gender = u.Gender.Value == UserGender.UGender.M ? 'M' : 'F',
                Role = u.Role.Value.ToString(),
                Birthday = u.Birthday
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserDto?> GetDtoByEmailAsync(string email)
    {
        return await _dbContext.Users
            .Where(u => u.Email == email)
            .Select(u=> new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Lastname = u.Lastname,
                Gender = u.Gender.Value == UserGender.UGender.M ? 'M' : 'F',
                Role = u.Role.Value.ToString(),
                Birthday = u.Birthday
            })
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ICollection<UserDto>> BrowseDtoUsers(int skip, int take)
    {
        return await _dbContext.Users
            .OrderBy(u => u.Id)
            .Skip(skip)
            .Take(take)
            .Select(u=> new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Lastname = u.Lastname,
                Gender = u.Gender.Value == UserGender.UGender.M ? 'M' : 'F',
                Role = u.Role.Value.ToString(),
                Birthday = u.Birthday
            })
            .ToListAsync();
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}