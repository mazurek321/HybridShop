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
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null) 
            throw new UserNotFoundException();
        
        var userGender = UserGender.FromChar(dto.Gender);
        
        user.UpdateProfile(dto.Name, dto.Lastname, userGender, dto.Birthday);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task SoftDeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null) 
            throw new UserNotFoundException();
        
        if(user.IsDeleted)
            throw new UserAlreadyDeletedException();

        user.DeleteUser();

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

    }



    //---------- Admin------------
    public async Task<User?> AdminGetUserDataAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if(user is null)
            throw new UserNotFoundException();

        return user;
    }
}