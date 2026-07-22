namespace BuildingBlocks.OpenApi.Context;

public interface IUserContext
{
    Guid Id { get; }
    string Role { get; }
    string Email { get; }
}