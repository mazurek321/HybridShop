using HybridShop.Services.Auth.Core.Models;

namespace HybridShop.Services.Auth.Core.Dto;

public class UserDto{
    public Guid Id { get; set ;}
    public string Email { get; set ;} = string.Empty;
    public string Name { get; set ;} = string.Empty;
    public string Lastname { get; set ;} = string.Empty;
    public char Gender { get; set ;}
    public UserRole Role { get; set ;}
    public  DateOnly Birthday { get; set ;}
}