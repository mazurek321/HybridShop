using HybridShop.Services.Auth.Application.Dto;
using HybridShop.Services.Auth.Application.Exceptions;
using HybridShop.Services.Auth.Core.Dto;
using HybridShop.Services.Auth.Core.Interfaces;
using HybridShop.Services.Auth.Core.Models;

namespace HybridShop.Services.Auth.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(
        IUserRepository userRepository
    )
    {
        _userRepository = userRepository;
    }

    private async Task<User> GetUser(Guid? userId, string? email, CancellationToken cancellationToken)
    {
        User? user = null;

        if (userId.HasValue)
            user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        else if (!string.IsNullOrWhiteSpace(email))
            user = await _userRepository.GetByEmailAsync(email, cancellationToken); 
        else
            throw new InvalidInputDataException();

        if (user is null) 
            throw new UserNotFoundException();

        return user;
    }

    public async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetUser(id, null, cancellationToken);
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await GetUser(null, email, cancellationToken);
    }

    public async Task<UserDto?> GetUserDataAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetDtoByIdAsync(id, cancellationToken);
        
        if(user is null)
            throw new UserNotFoundException();

        return user;
    }

    public async Task<ICollection<UserDto>> BrowseUsersAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        if(skip < 0 || take < 0 || take > 100)
            throw new InvalidRangeException();

        return await _userRepository.BrowseDtoUsers(skip, take, cancellationToken);
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await GetUser(id, null, cancellationToken);
        
        var userGender = UserGender.FromChar(dto.Gender);
        
        user.UpdateProfile(dto.Name, dto.Lastname, userGender, dto.Birthday);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetUser(id, null, cancellationToken);
        
        if(user.IsDeleted)
            throw new UserAlreadyDeletedException();

        user.DeleteUser();

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<AdminUserDto?> AdminGetUserDataAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAdminDtoByIdAsync(id, cancellationToken);
        
        if(user is null)
            throw new UserNotFoundException();

        return user;
    }

    public async Task<ICollection<AdminUserDto>> AdminBrowseUsersAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        if(skip < 0 || take < 0 || take > 100)
            throw new InvalidRangeException();

        return await _userRepository.BrowseAdminDtoUsers(skip, take, cancellationToken);
    }

    public async Task AdminUpdateUserAsync(Guid? userId, string? email, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await GetUser(userId, email, cancellationToken);
        
        var userGender = UserGender.FromChar(dto.Gender);
        
        user.UpdateProfile(dto.Name, dto.Lastname, userGender, dto.Birthday);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task AdminSoftDeleteUserAsync(Guid? userId, string? email, CancellationToken cancellationToken = default)
    {
        var user = await GetUser(userId, email, cancellationToken);
        
        if(user.IsDeleted)
            throw new UserAlreadyDeletedException();

        user.DeleteUser();

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task AdminBanUser(Guid? userId, string? email, CancellationToken cancellationToken = default)
    {
        var user = await GetUser(userId, email, cancellationToken);
        
        if(user.IsBanned)
            throw new UserAlreadyBannedException();

        user.BanUser();

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task AdminUnbanUser(Guid? userId, string? email, CancellationToken cancellationToken = default)
    {
        var user = await GetUser(userId, email, cancellationToken);
        
        if(!user.IsBanned)
            throw new UserIsNotBannedException();

        user.UnbanUser();

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}