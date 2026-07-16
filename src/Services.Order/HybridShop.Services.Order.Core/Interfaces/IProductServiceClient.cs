using HybridShop.Services.Order.Core.Models.Dto;

namespace HybridShop.Services.Order.Core.Interfaces;
public interface IProductServiceClient
{
    Task<List<ProductExternalDto>> GetProductsByIdsAsync(IEnumerable<Guid> productIds);
}