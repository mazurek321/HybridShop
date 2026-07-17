using HybridShop.Services.Order.Application.Dto;
using HybridShop.Services.Order.Application.Exceptions;
using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.Dto;
using HybridShop.Services.Order.Core.Models.Order;

namespace HybridShop.Services.Order.Application.Services;

public class OrderService
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceClient _productClient;

    public OrderService(
        IShoppingCartRepository cartRepository,
        IOrderRepository orderRepository,
        IProductServiceClient productClient
    )
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _productClient = productClient;
    }

    public async Task<List<OrderDto>> CreateOrdersFromCartAsync(Guid userId, CreateOrderDto dto)
    {
        var cart = await _cartRepository.GetCartAsync(userId);
        if (cart is null || !cart.Items.Any())
            throw new CartIsEmptyOrDoesntExistException();

        if (cart.Delivery is null)
            throw new InvalidDeliveryAddressException();

        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var productsDict = new Dictionary<Guid, ProductExternalDto>();

        foreach (var id in productIds)
        {
            var p = await _productClient.GetProductBySkuIdAsync(id) ?? 
                    (await _productClient.GetProductsByIdsAsync(new[] { id })).FirstOrDefault();
            if (p is not null)
            {
                productsDict[id] = p;
            }
        }
        
        var itemsBySeller = cart.Items.GroupBy(item => 
            productsDict.TryGetValue(item.ProductId, out var p) ? p.SellerId : Guid.Empty);

        var ordersToCreate = new List<Core.Models.Order.Order>();

        foreach (var group in itemsBySeller)
        {
            var sellerId = group.Key;
            var orderItems = group.Select(i => OrderItem.AddOrderItem(
                i.ProductId,
                productsDict.TryGetValue(i.ProductId, out var p) ? p.Title : "Produkt",
                i.Quantity.Value,
                i.Price
            )).ToList();

            var order = Core.Models.Order.Order.Create(
                userId,
                sellerId,
                orderItems,
                cart.Delivery.Price,
                cart.Delivery.Name,
                dto.ShippingAddress
            );

            await _orderRepository.AddAsync(order);
            ordersToCreate.Add(order);
        }

        await _cartRepository.DeleteCartAsync(userId);

        return ordersToCreate.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<OrderDto>> GetBuyerOrdersAsync(Guid buyerId)
    {
        var orders = await _orderRepository.GetByBuyerIdAsync(buyerId);
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetSellerOrdersAsync(Guid sellerId)
    {
        var orders = await _orderRepository.GetBySellerIdAsync(sellerId);
        return orders.Select(MapToDto);
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
            throw new OrderNotFoundException();

        order.UpdateStatus(status);
        await _orderRepository.UpdateAsync(order);
    }

    private static OrderDto MapToDto(Core.Models.Order.Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            BuyerId = order.BuyerId,
            SellerId = order.SellerId,
            DeliveryPrice = order.DeliveryPrice,
            DeliveryName = order.DeliveryName,
            Total = order.Total,
            ShippingAddress = order.ShippingAddress,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                Title = i.Title,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
    }
}