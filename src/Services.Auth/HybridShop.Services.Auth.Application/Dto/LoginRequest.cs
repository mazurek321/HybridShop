namespace HybridShop.Services.Auth.Application.Dto;
public record LoginRequest(
    string Email, 
    string Password
);