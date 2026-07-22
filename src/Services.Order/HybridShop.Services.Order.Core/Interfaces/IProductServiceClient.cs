using HybridShop.Services.Order.Core.Models.Dto;

namespace HybridShop.Services.Order.Core.Interfaces;

public interface IProductServiceClient
{
    Task<List<ProductExternalDto>> GetProductsByIdsAsync(IEnumerable<(Guid ProductId, Guid? SkuId)> items, CancellationToken cancellationToken = default);
    Task<ProductExternalDto?> GetProductBySkuIdAsync(Guid productId, Guid? skuId, CancellationToken cancellationToken = default);
}