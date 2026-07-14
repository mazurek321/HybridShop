using HybridShop.Services.Product.Core.Dto;

namespace HybridShop.Services.Product.Core.Interfaces;

public interface IUserServiceClient
{
    Task<SellerDto?> GetSellerDetailsAsync(Guid sellerId);
}