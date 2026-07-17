using HybridShop.Services.Order.Core.Models.Dto;

namespace HybridShop.Services.Order.Core.Interfaces;

public interface IProductServiceClient
{
    Task<IEnumerable<ProductExternalDto>> GetProductsByIdsAsync(IEnumerable<Guid> ids);
    Task<ProductExternalDto?> GetProductBySkuIdAsync(Guid skuId);
}