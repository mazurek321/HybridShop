namespace HybridShop.Services.Auth.Application.Dto;

public record AuthResponse(
    string AccessToken, 
    string RefreshToken
);