namespace HybridShop.Services.Auth.Application.Dto;

public record AdminUserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Lastname { get; init; } = string.Empty;
    public char Gender { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateOnly Birthday { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsDeleted { get; init; }
    public bool IsBanned { get; init; }
    public ICollection<UserSessionDto> ActiveSessions { get; init; } = [];
}

public record UserSessionDto
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsActive { get; init; }
}