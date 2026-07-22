using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using HybridShop.Services.Product.Core.Interfaces;

namespace HybridShop.Services.Product.Infrastructure.Services;

public class MinioStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };

    public MinioStorageService(IConfiguration configuration)
    {
        var endpoint = string.IsNullOrWhiteSpace(configuration["Minio:Endpoint"]) 
            ? "minio:9000" 
            : configuration["Minio:Endpoint"]!;

        var accessKey = string.IsNullOrWhiteSpace(configuration["Minio:AccessKey"]) 
            ? (configuration["MINIO_ROOT_USER"] ?? "minioadmin") 
            : configuration["Minio:AccessKey"]!;

        var secretKey = string.IsNullOrWhiteSpace(configuration["Minio:SecretKey"]) 
            ? (configuration["MINIO_ROOT_PASSWORD"] ?? "minioadminpassword") 
            : configuration["Minio:SecretKey"]!;

        _bucketName = string.IsNullOrWhiteSpace(configuration["Minio:BucketName"]) 
            ? "product-images" 
            : configuration["Minio:BucketName"]!;

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }

    public async Task<string> UploadFileAsync(Guid productId, IFormFile file, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension) || !AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            throw new ArgumentException($"Niedozwolony format pliku. Akceptowane formaty: {string.Join(", ", AllowedExtensions)}");
        }

        var objectKey = $"{productId}/{Guid.NewGuid()}{extension}";

        using var stream = file.OpenReadStream();
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType), cancellationToken);

        return $"http://localhost:9000/{_bucketName}/{objectKey}";
    }

    public async Task DeleteFileAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) return;

        var uri = new Uri(imageUrl);
        var objectKey = uri.AbsolutePath.TrimStart('/').Replace($"{_bucketName}/", "");

        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey), cancellationToken);
    }
}