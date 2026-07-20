using Grpc.Core;
using HybridShop.Grpc;
using HybridShop.Services.Product.Core.Dto;
using HybridShop.Services.Product.Core.Interfaces;

namespace HybridShop.Services.Product.Infrastructure.Clients;

public class UserServiceClient : IUserServiceClient
{
    private readonly UserGrpcService.UserGrpcServiceClient _grpcClient;

    public UserServiceClient(UserGrpcService.UserGrpcServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<SellerDto?> GetSellerDetailsAsync(Guid sellerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new UserRequest 
            { 
                Id = sellerId.ToString() 
            };

            var response = await _grpcClient.GetUserDetailsAsync(
                request, 
                cancellationToken: cancellationToken
            );

            return new SellerDto
            {
                Id = Guid.Parse(response.Id),
                Email = response.Email,
                Name = response.Name,
                Lastname = response.Lastname 
            };
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"[gRPC Client Error]: {ex.Status.Detail} (Code: {ex.StatusCode})");
            return null;
        }
    }
}