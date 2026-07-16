using HybridShop.Services.Order.Application.Dto;
using HybridShop.Services.Order.Application.Exceptions;
using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.ShoppingCart;

namespace HybridShop.Services.Order.Application.Services;

public class ShoppingCartService
{
    private readonly IShoppingCartRepository _repository;
    public ShoppingCartService(IShoppingCartRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShoppingCartDto> GetCartAsync(Guid userId)
    {
        var cart = await _repository.GetCartAsync(userId);
        if (cart is null)
        {
            var newCart = ShoppingCart.NewShoppingCart(userId);
            await _repository.UpdateCartAsync(newCart);
            return MapToDto(newCart);
        }

        return MapToDto(cart);
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

    private static ShoppingCartDto MapToDto(ShoppingCart cart)
    {
        return new ShoppingCartDto
        {
            UserId = cart.UserId,
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity.Value
            }).ToList()
        };
    }
}