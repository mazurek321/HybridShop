using HybridShop.Services.Auth.Application.Dto;
using HybridShop.Services.Auth.Core.Models;
using HybridShop.Services.Auth.Core.Interfaces;
using HybridShop.Services.Auth.Application.Exceptions;

namespace HybridShop.Services.Auth.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        TokenService tokenService
    )
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var exists = await _userRepository.ExistsAsync(request.Email);
        if (exists)
            throw new EmailAlreadyExistsException(request.Email);
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var userGender = UserGender.FromChar(request.Gender);

        var user = User.NewUser(
            request.Email,
            hashedPassword,
            request.Name,
            request.Lastname,
            userGender,
            request.Birthday
        );

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) 
            throw new InvalidCredentialsException();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenString = _tokenService.GenerateRefreshTokenString();

        user.AddRefreshToken(refreshTokenString, TimeSpan.FromDays(7));
        
        await _userRepository.SaveChangesAsync();

        return new AuthResponse(accessToken, refreshTokenString);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

        if (user == null) 
            throw new UserNotFoundException();

        var activeToken = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken);
        if (activeToken == null || !activeToken.IsActive) 
            throw new InvalidTokenException();

        user.RevokeRefreshToken(request.RefreshToken);

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenString = _tokenService.GenerateRefreshTokenString();

        user.AddRefreshToken(newRefreshTokenString, TimeSpan.FromDays(7));
        
        await _userRepository.SaveChangesAsync();

        return new AuthResponse(newAccessToken, newRefreshTokenString);
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null) 
            throw new UserNotFoundException();

        user.RevokeAllRefreshTokens();
        
        await _userRepository.SaveChangesAsync();
    }
}