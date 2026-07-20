using HybridShop.Services.Order.Core.Models.Dto;

namespace HybridShop.Services.Order.Core.Interfaces;

public interface IProductServiceClient
{
    Task<IEnumerable<ProductExternalDto>> GetProductsByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<ProductExternalDto?> GetProductBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default);
}