using HybridShop.Services.Auth.Core.Dto;
using HybridShop.Services.Auth.Core.Interfaces;

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
            throw new Exception("User not found.");

        return user;
    }
}