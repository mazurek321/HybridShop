using Grpc.Core;
using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.Dto;
using HybridShop.Services.Product.Grpc;
using Microsoft.Extensions.Logging;

namespace HybridShop.Services.Order.Infrastructure.Clients;

public class ProductServiceClient : IProductServiceClient
{
    private readonly ProductGrpcService.ProductGrpcServiceClient _grpcClient;
    private readonly ILogger<ProductServiceClient> _logger;

    public ProductServiceClient(
        ProductGrpcService.ProductGrpcServiceClient grpcClient,
        ILogger<ProductServiceClient> logger)
    {
        _grpcClient = grpcClient;
        _logger = logger;
    }

    public async Task<List<ProductExternalDto>> GetProductsByIdsAsync(
        IEnumerable<(Guid ProductId, Guid? SkuId)> items, 
        CancellationToken cancellationToken = default)
    {
        var result = new List<ProductExternalDto>();
        var validItems = items?.Where(i => i.ProductId != Guid.Empty).ToList();

        Console.WriteLine($"\n[gRPC Client] Przygotowywanie żądania do ProductService. Liczba przekazanych pozycji: {validItems?.Count ?? 0}");

        if (validItems is null || !validItems.Any())
        {
            Console.WriteLine("[gRPC Client] Brak prawidłowych ID produktów – pomijanie strzału gRPC.");
            return result;
        }

        try
        {
            var request = new GetProductsByIdsRequest();
            
            foreach (var item in validItems)
            {
                var protoItem = new ProductItemRequest
                {
                    ProductId = item.ProductId.ToString(),
                    SkuId = item.SkuId.HasValue ? item.SkuId.Value.ToString() : string.Empty
                };
                request.Items.Add(protoItem);
                Console.WriteLine($"[gRPC Client] Dodano do żądania -> ProductId: {protoItem.ProductId}, SkuId: '{protoItem.SkuId}'");
            }

            Console.WriteLine("[gRPC Client] Wysyłanie żądania gRPC...");
            var response = await _grpcClient.GetProductsByIdsAsync(request, cancellationToken: cancellationToken);

            Console.WriteLine($"[gRPC Client] Otrzymano odpowiedź gRPC. Liczba zwróconych produktów: {response?.Products?.Count ?? 0}");

            if (response?.Products is null || !response.Products.Any())
                return result;

            foreach (var item in response.Products)
            {
                if (!Guid.TryParse(item.ProductId, out var parsedProductId))
                {
                    Console.WriteLine($"[gRPC Client] OSTRZEŻENIE: Nie udało się sparsować ProductId z odpowiedzi: '{item.ProductId}'");
                    continue;
                }

                Guid.TryParse(item.SkuId, out var parsedSkuId);
                Guid.TryParse(item.SellerId, out var parsedSellerId);

                var dto = new ProductExternalDto
                {
                    ProductId = parsedProductId,
                    SkuId = parsedSkuId != Guid.Empty ? parsedSkuId : null,
                    Title = item.Title ?? string.Empty,
                    Price = (decimal)item.Price,
                    Quantity = item.Quantity,
                    SellerId = parsedSellerId
                };

                Console.WriteLine($"[gRPC Client] Zmapowano produkt z gRPC -> ProductId: {dto.ProductId}, SkuId: {dto.SkuId?.ToString() ?? "brak"}, Price: {dto.Price}, Title: '{dto.Title}'");
                result.Add(dto);
            }
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"[gRPC Client] BŁĄD gRPC: Status={ex.StatusCode}, Detail='{ex.Status.Detail}'");
            _logger.LogError(ex, "Błąd gRPC podczas pobierania produktów: Status={StatusCode}, Detail={Detail}", 
                ex.StatusCode, ex.Status.Detail);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[gRPC Client] BŁĄD NIEOCZEKIWANY: {ex.Message}");
            _logger.LogError(ex, "Nieoczekiwany błąd podczas komunikacji z ProductService");
            throw;
        }

        Console.WriteLine("[gRPC Client] Zakończono przetwarzanie żądania z powodzeniem.\n");
        return result;
    }

    public async Task<ProductExternalDto?> GetProductBySkuIdAsync(
        Guid productId, 
        Guid? skuId, 
        CancellationToken cancellationToken = default)
    {
        var list = await GetProductsByIdsAsync(new[] { (productId, skuId) }, cancellationToken);
        return list.FirstOrDefault();
    }
}