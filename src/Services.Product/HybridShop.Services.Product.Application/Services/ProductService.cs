using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Application.Exceptions;
using HybridShop.Services.Product.Core.Dto;
using HybridShop.Services.Product.Core.Exceptions;
using HybridShop.Services.Product.Core.Interfaces;
using HybridShop.Services.Product.Core.Product;
using Microsoft.AspNetCore.Http;

namespace HybridShop.Services.Product.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserServiceClient _userServiceClient;
    private readonly IFileStorageService _fileStorageService;

    public ProductService(
        IProductRepository productRepository,
        IUserServiceClient userServiceClient,
        IFileStorageService fileStorageService
    )
    {
        _productRepository = productRepository;
        _userServiceClient = userServiceClient;
        _fileStorageService = fileStorageService;
    }

    public async Task<Core.Product.Product> AddProductAsync(AddNewProductDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        var slug = GenerateSlug(dto.Title);

        if (!Enum.TryParse<PCategory>(dto.Category, true, out var parsedCategory))
            throw new InvalidProductException();

        var domainPrice = new Price(dto.Price);
        var domainQuantity = new Quantity(dto.Quantity);
        var domainVariants = MapVariants(dto.Variants);

        var product = Core.Product.Product.NewProduct(
            dto.Title,
            slug,
            dto.Description,
            domainPrice,
            domainQuantity,
            userId,
            new ProductCategory(parsedCategory),
            dto.Attributes,
            domainVariants
        );

        await _productRepository.AddAsync(product, cancellationToken);
        return product;
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new ProductNotFoundException();

        var seller = await _userServiceClient.GetSellerDetailsAsync(product.SellerId, cancellationToken);

        return MapToDto(product, seller);
    }

    public async Task<IEnumerable<ProductDto>> BrowseProductsAsync(BrowseProductsQueryDto query, CancellationToken cancellationToken = default)
    {
        PCategory? parsedCategory = null;
        if (!string.IsNullOrWhiteSpace(query.Category) && Enum.TryParse<PCategory>(query.Category, true, out var categoryVal))
        {
            parsedCategory = categoryVal;
        }

        var products = await _productRepository.BrowseProductsAsync(
            query.Skip, 
            query.Take, 
            parsedCategory, 
            query.PriceFrom, 
            query.PriceTo, 
            query.Search,
            cancellationToken: cancellationToken
        );

        var tasks = products.Select(async product =>
        {
            var seller = await _userServiceClient.GetSellerDetailsAsync(product.SellerId, cancellationToken);
            return MapToDto(product, seller);
        });

        return await Task.WhenAll(tasks);
    }

    public async Task<List<string>> AddImagesToProductAsync(Guid userId, Guid productId, List<IFormFile> files, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new ProductNotFoundException();
        
        if (product.SellerId != userId)
            throw new DontHavePermissionsException();

        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            var imageUrl = await _fileStorageService.UploadFileAsync(productId, file, cancellationToken);
            product.AddImage(imageUrl);
            uploadedUrls.Add(imageUrl);
        }

        await _productRepository.UpdateAsync(product, cancellationToken);
        return uploadedUrls;
    }

    public async Task<List<string>> AddImagesToVariantAsync(Guid userId, Guid productId, Guid skuId, List<IFormFile> files, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new ProductNotFoundException();
        
        if (product.SellerId != userId)
            throw new DontHavePermissionsException();

        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            var imageUrl = await _fileStorageService.UploadFileAsync(productId, file, cancellationToken);
            product.AddVariantImage(skuId, imageUrl);
            uploadedUrls.Add(imageUrl);
        }

        await _productRepository.UpdateAsync(product, cancellationToken);
        return uploadedUrls;
    }

    public async Task DeleteProductImageAsync(Guid userId, Guid productId, string imageUrl, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new ProductNotFoundException();

        if (product.SellerId != userId)
            throw new DontHavePermissionsException();

        await _fileStorageService.DeleteFileAsync(imageUrl, cancellationToken);

        product.RemoveImage(imageUrl);
        await _productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task DeleteVariantImageAsync(Guid userId, Guid productId, Guid skuId, string imageUrl, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new ProductNotFoundException();

        if (product.SellerId != userId)
            throw new DontHavePermissionsException();

        await _fileStorageService.DeleteFileAsync(imageUrl, cancellationToken);

        product.RemoveVariantImage(skuId, imageUrl);
        await _productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task UpdateProduct(Guid productId, Guid userId, string role, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);

        if (product is null)
            throw new ProductNotFoundException();

        if (product.SellerId != userId && role != "Admin")
            throw new DontHavePermissionsException();
        
        if (!Enum.TryParse<PCategory>(dto.Category, true, out var parsedCategory))
            throw new InvalidProductException();

        var domainPrice = new Price(dto.Price);
        var domainQuantity = new Quantity(dto.Quantity);
        var domainVariants = MapVariants(dto.Variants);
        var newSlug = GenerateSlug(dto.Title);

        product.Update(
            dto.Title,
            newSlug,
            dto.Description,
            domainPrice,
            domainQuantity,
            new ProductCategory(parsedCategory),
            dto.Attributes,
            domainVariants
        );

        await _productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task DeleteProduct(Guid productId, Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);

        if (product is null)
            throw new ProductNotFoundException();

        if (product.SellerId != userId && role != "Admin")
            throw new DontHavePermissionsException();

        foreach (var img in product.Images)
        {
            await _fileStorageService.DeleteFileAsync(img, cancellationToken);
        }

        foreach (var variant in product.Variants)
        {
            foreach (var img in variant.Images)
            {
                await _fileStorageService.DeleteFileAsync(img, cancellationToken);
            }
        }

        product.Delete();
        await _productRepository.UpdateAsync(product, cancellationToken);
    }

    private static ProductDto MapToDto(Core.Product.Product product, SellerDto? seller)
    {
        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title ?? string.Empty,
            Slug = product.Slug ?? string.Empty,
            Description = product.Description ?? string.Empty,
            Price = product.Price?.Value ?? 0, 
            Quantity = product.Quantity?.Value ?? 0,
            Category = product.Category?.Value.ToString() ?? string.Empty,
            Attributes = product.Attributes ?? new Dictionary<string, object>(),
            Images = product.Images ?? new List<string>(),
            Variants = (product.Variants ?? new List<ProductVariant>()).Select(v => new ProductVariantDto
            {
                SkuId = v.SkuId,
                Price = v.Price?.Value ?? 0,
                Quantity = v.Quantity?.Value ?? 0,
                Attributes = v.Attributes ?? new Dictionary<string, object>(),
                Images = v.Images ?? new List<string>()
            }).ToList(),
            Seller = seller ?? new SellerDto
            {
                Id = product.SellerId,
                Email = "unknown@shop.local",
                Name = "Nieznany",
                Lastname = "Sprzedawca"
            },
            IsDeleted = product.IsDeleted,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    private static List<ProductVariant> MapVariants(List<AddProductVariantDto>? dtos)
    {
        return dtos is not null && dtos.Any()
            ? dtos.Select(v => new ProductVariant(Guid.NewGuid(), new Price(v.Price), new Quantity(v.Quantity), v.Attributes)).ToList()
            : new List<ProductVariant>();
    }

    private static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        string str = title.ToLowerInvariant().Trim();

        str = str.Replace("ą", "a").Replace("ć", "c").Replace("ę", "e")
                .Replace("ł", "l").Replace("ń", "n").Replace("ó", "o")
                .Replace("ś", "s").Replace("ź", "z").Replace("ż", "z");

        System.Text.StringBuilder sb = new();
        foreach (char c in str)
        {
            if (char.IsLetterOrDigit(c) || c == ' ' || c == '-')
            {
                sb.Append(c);
            }
        }
        str = sb.ToString();

        str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", "-");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"-+", "-");

        return str;
    }
}