namespace HybridShop.Services.Auth.Core.Interfaces;

using HybridShop.Services.Auth.Core.Dto;
using HybridShop.Services.Auth.Core.Models;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<UserDto?> GetDtoByIdAsync(Guid id);
    Task<UserDto?> GetDtoByEmailAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<ICollection<UserDto>> BrowseDtoUsers(int skip, int take);
    Task<bool> ExistsAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task SaveChangesAsync();
}