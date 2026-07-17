using HybridShop.Services.Order.Application.Dto;
using HybridShop.Services.Order.Application.Exceptions;
using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.ShoppingCart;
using HybridShop.Services.Order.Core.Models.Dto;
using HybridShop.Services.Order.Core.Models.Delivery;
using HybridShop.Services.Order.Core.Exceptions;

namespace HybridShop.Services.Order.Application.Services;

public class ShoppingCartService
{
    private readonly IShoppingCartRepository _repository;
    private readonly IProductServiceClient _productClient; 

    public ShoppingCartService(
        IShoppingCartRepository repository,
        IProductServiceClient productClient
    )
    {
        _repository = repository;
        _productClient = productClient;
    }

    public async Task<ShoppingCartDto> GetCartAsync(Guid userId)
    {
        var cart = await _repository.GetCartAsync(userId);
        if (cart is null)
        {
            var newCart = ShoppingCart.NewShoppingCart(userId);
            await _repository.UpdateCartAsync(newCart);
            return new ShoppingCartDto { UserId = userId, CartVersion = newCart.Version };
        }

        if (!cart.Items.Any())
        {
            return new ShoppingCartDto { UserId = cart.UserId, CartVersion = cart.Version };
        }

        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var productsDict = new Dictionary<Guid, ProductExternalDto>();

        foreach (var id in productIds)
        {
            var productDetails = await _productClient.GetProductBySkuIdAsync(id);
            
            if (productDetails is null)
            {
                var mainProducts = await _productClient.GetProductsByIdsAsync(new[] { id });
                productDetails = mainProducts.FirstOrDefault();
            }

            if (productDetails is not null)
            {
                productsDict[id] = productDetails;
            }
        }

        return new ShoppingCartDto
        {
            UserId = cart.UserId,
            CartVersion = cart.Version,
            Total = cart.Total,
            DeliveryCost = cart.Delivery?.Price ?? 0m,
            DeliveryName = cart.Delivery?.Name,
            Items = cart.Items.Select(i =>
            {
                productsDict.TryGetValue(i.ProductId, out var productDetails);

                return new CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity.Value,
                    Title = productDetails?.Title ?? "Produkt niedostępny",
                    Price = i.Price
                };
            }).ToList()
        };
    }

    public async Task AddItemToCartAsync(Guid userId, AddCartItemDto dto)
    {
        ProductExternalDto? product = await _productClient.GetProductBySkuIdAsync(dto.ProductId);

        if (product is null)
        {
            var externalProducts = await _productClient.GetProductsByIdsAsync(new[] { dto.ProductId });
            product = externalProducts.FirstOrDefault();
        }

        if (product is null)
            throw new ProductNotFoundException(dto.ProductId);

        var cart = await _repository.GetCartAsync(userId) ?? ShoppingCart.NewShoppingCart(userId);
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        
        int currentQuantityInCart = existingItem?.Quantity.Value ?? 0;
        int targetQuantity = currentQuantityInCart + dto.Quantity;

        if (targetQuantity > product.Quantity)
            throw new Exceptions.InvalidQuantityException();

        cart.AddItem(
            dto.ProductId, 
            new Quantity(targetQuantity),
            product.Price
        );

        await _repository.UpdateCartAsync(cart);
    }

    public async Task RemoveItemFromCartAsync(Guid userId, Guid productId)
    {
        var cart = await _repository.GetCartAsync(userId);
        if (cart is null)
            throw new CartNotFoundException();

        cart.RemoveItem(productId);
        await _repository.UpdateCartAsync(cart);
    }

    public async Task ClearCartAsync(Guid userId)
    {
        var cart = await _repository.GetCartAsync(userId);
        if (cart is null)
            throw new CartNotFoundException();

        await _repository.DeleteCartAsync(userId);
    }

    public async Task SetDeliveryMethodAsync(Guid userId, SetDeliveryMethodDto dto)
    {
        if (!Enum.IsDefined(typeof(DeliveryType), dto.DeliveryTypeId))
            throw new InvalidDeliveryTypeException();

        var cart = await _repository.GetCartAsync(userId);
        if (cart is null)
            throw new CartNotFoundException();

        var option = DeliveryOption.Create((DeliveryType)dto.DeliveryTypeId);
        var delivery = DeliveryMethod.ChooseDelivery(option.Name, option.Price);
        cart.SetDeliveryMethod(delivery);

        await _repository.UpdateCartAsync(cart);
    }

    public Task<List<DeliveryOptionDto>> GetAvailableDeliveryMethodsAsync()
    {
        var options = Enum.GetValues<DeliveryType>()
            .Select(type => 
            {
                var option = DeliveryOption.Create(type);
                return new DeliveryOptionDto
                {
                    Id = (int)option.Type,
                    Name = option.Name,
                    Price = option.Price
                };
            })
            .ToList();

        return Task.FromResult(options);
    }
}