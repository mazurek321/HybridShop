namespace HybridShop.Services.Auth.Core.Interfaces;

using HybridShop.Services.Auth.Application.Dto;
using HybridShop.Services.Auth.Core.Dto;
using HybridShop.Services.Auth.Core.Models;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<UserDto?> GetDtoByIdAsync(Guid id);
    Task<AdminUserDto?> GetAdminDtoByIdAsync(Guid id);
    Task<UserDto?> GetDtoByEmailAsync(string email);
    Task<AdminUserDto?> GetAdminDtoByEmailAsync(string email);
    Task<User?> GetWithTokensByEmailAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<ICollection<UserDto>> BrowseDtoUsers(int skip, int take);
    Task<ICollection<AdminUserDto>> BrowseAdminDtoUsers(int skip, int take);
    void DeleteRefreshToken(RefreshToken token);
    Task<bool> ExistsAsync(string email);
    void Add(User user);
    void Update(User user);
    Task SaveChangesAsync();
}