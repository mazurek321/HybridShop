using Microsoft.AspNetCore.Http;

namespace HybridShop.Services.Product.Core.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Guid productId, IFormFile file, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string imageUrl, CancellationToken cancellationToken = default);
}