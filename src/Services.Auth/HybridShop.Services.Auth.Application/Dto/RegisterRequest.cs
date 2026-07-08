using HybridShop.Services.Auth.Core.Models;

namespace HybridShop.Services.Auth.Application.Dto;

public record RegisterRequest(
    string Email, 
    string Password, 
    string Name, 
    string Lastname, 
    char Gender, 
    UserRole Role, 
    DateOnly Birthday
);