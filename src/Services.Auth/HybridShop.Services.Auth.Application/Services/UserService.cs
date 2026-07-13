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

    private async Task<User> GetUser(Guid? userId, string? email)
    {
        User? user = null;

        if (userId.HasValue)
            user = await _userRepository.GetByIdAsync(userId.Value);
        else if (!string.IsNullOrWhiteSpace(email))
            user = await _userRepository.GetByEmailAsync(email); 
        else
            throw new InvalidInputDataException();

        if (user is null) 
            throw new UserNotFoundException();

        return user;
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        return await GetUser(id, null);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await GetUser(null, email);
    }

    public async Task<UserDto?> GetUserDataAsync(Guid id)
    {
        var user = await _userRepository.GetDtoByIdAsync(id);
        
        if(user is null)
            throw new UserNotFoundException();

        return user;
    }

    public async Task <ICollection<UserDto>> BrowseUsersAsync(int skip, int take)
    {
        if(skip < 0 || take < 0 || take > 100)
            throw new InvalidRangeException();

        return await _userRepository.BrowseDtoUsers(skip, take);
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await GetUser(id, null);
        
        var userGender = UserGender.FromChar(dto.Gender);
        
        user.UpdateProfile(dto.Name, dto.Lastname, userGender, dto.Birthday);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task SoftDeleteUserAsync(Guid id)
    {
        var user = await GetUser(id, null);
        
        if(user.IsDeleted)
            throw new UserAlreadyDeletedException();

        user.DeleteUser();

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

    }



    //---------- Admin------------
    public async Task<AdminUserDto?> AdminGetUserDataAsync(Guid id)
    {
        var user = await _userRepository.GetAdminDtoByIdAsync(id);
        
        if(user is null)
            throw new UserNotFoundException();

        return user;
    }

    public async Task <ICollection<AdminUserDto>> AdminBrowseUsersAsync(int skip, int take)
    {
        if(skip < 0 || take < 0 || take > 100)
            throw new InvalidRangeException();

        return await _userRepository.BrowseAdminDtoUsers(skip, take);
    }

    public async Task AdminUpdateUserAsync(Guid? userId, string? email, UpdateUserDto dto)
    {
        var user = await GetUser(userId, email);
        
        var userGender = UserGender.FromChar(dto.Gender);
        
        user.UpdateProfile(dto.Name, dto.Lastname, userGender, dto.Birthday);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task AdminSoftDeleteUserAsync(Guid? userId, string? email)
    {
        var user = await GetUser(userId, email);
        
        if(user.IsDeleted)
            throw new UserAlreadyDeletedException();

        user.DeleteUser();

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task AdminBanUser(Guid? userId, string? email)
    {
        var user = await GetUser(userId, email);
        
        if(user.IsBanned)
            throw new UserAlreadyBannedException();

        user.BanUser();

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

    }

    public async Task AdminUnbanUser(Guid? userId, string? email)
    {
        var user = await GetUser(userId, email);
        
        if(!user.IsBanned)
            throw new UserIsNotBannedException();

        user.UnbanUser();

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

    }

}