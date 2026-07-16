using HybridShop.Services.Order.Application.Dto;
using HybridShop.Services.Order.Application.Exceptions;
using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.ShoppingCart;

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
            return new ShoppingCartDto { UserId = userId };
        }

        if (!cart.Items.Any())
        {
            return new ShoppingCartDto { UserId = cart.UserId };
        }

        var productIds = cart.Items.Select(i => i.ProductId).ToList();

        var externalProducts = await _productClient.GetProductsByIdsAsync(productIds);
        var productsDict = externalProducts.ToDictionary(p => p.Id);

        return new ShoppingCartDto
        {
            UserId = cart.UserId,
            Items = cart.Items.Select(i =>
            {
                productsDict.TryGetValue(i.ProductId, out var productDetails);

                return new CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity.Value,
                    Title = productDetails?.Title ?? "Produkt niedostępny",
                    Price = productDetails?.Price ?? 0m
                };
            }).ToList()
        };
    }

    public async Task AddItemToCartAsync(Guid userId, AddCartItemDto dto)
    {
        var cart = await _repository.GetCartAsync(userId) ?? ShoppingCart.NewShoppingCart(userId);

        cart.AddItem(
            dto.ProductId, 
            new Quantity(dto.Quantity)
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
}