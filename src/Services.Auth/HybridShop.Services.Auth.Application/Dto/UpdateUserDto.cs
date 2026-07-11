using HybridShop.Services.Auth.Core.Models;

namespace HybridShop.Services.Auth.Application.Dto;

public record UpdateUserDto(
    string Name, 
    string Lastname, 
    char Gender, 
    DateOnly Birthday
);