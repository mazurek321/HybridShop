using Grpc.Core;
using HybridShop.Grpc;
using HybridShop.Services.Auth.Application.Services;

namespace HybridShop.Services.Auth.Api.Grpc;

public class UserGrpcServer : UserGrpcService.UserGrpcServiceBase
{
    private readonly UserService _userService;

    public UserGrpcServer(UserService userService)
    {
        _userService = userService;
    }
    public override async Task<UserResponse> GetUserDetails(UserRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out Guid userId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Identyfikator użytkownika ma nieprawidłowy format."));

        var user = await _userService.GetUserDataAsync(userId);

        if (user is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Użytkownik o ID {userId} nie został znaleziony."));

        return new UserResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            Name = user.Name,       
            Lastname = user.Lastname     
        };
    }
}