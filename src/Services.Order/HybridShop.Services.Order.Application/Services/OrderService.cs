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
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IShoppingCartRepository cartRepository,
        IOrderRepository orderRepository,
        IProductServiceClient productClient,
        IUnitOfWork unitOfWork
    )
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _productClient = productClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<OrderDto>> CreateOrdersFromCartAsync(Guid userId, CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartAsync(userId, cancellationToken);
        if (cart is null || !cart.Items.Any())
            throw new CartIsEmptyOrDoesntExistException();

        if (cart.Version != dto.CartVersion)
            throw new CartConcurrencyException();

        if (cart.Delivery is null)
            throw new InvalidDeliveryAddressException();

        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var productsDict = new Dictionary<Guid, ProductExternalDto>();

        foreach (var id in productIds)
        {
            var productDetails = await _productClient.GetProductBySkuIdAsync(id, cancellationToken);
            
            if (productDetails is null)
            {
                var mainProducts = await _productClient.GetProductsByIdsAsync(new[] { id }, cancellationToken);
                productDetails = mainProducts.FirstOrDefault();
            }

            if (productDetails is not null)
            {
                productsDict[id] = productDetails;
            }
        }

        foreach (var item in cart.Items)
        {
            if (!productsDict.TryGetValue(item.ProductId, out var product))
                throw new ProductNotFoundException(item.ProductId);

            if (product.Price != item.Price)
                throw new InvalidPriceException();
                
            if (product.Quantity < item.Quantity.Value)
                throw new InvalidQuantityException();
        }

        var finalCheckCart = await _cartRepository.GetCartAsync(userId, cancellationToken);
        if (finalCheckCart is null || finalCheckCart.Version != dto.CartVersion)
            throw new CartConcurrencyException();

        await _cartRepository.DeleteCartAsync(userId, cancellationToken);

        var itemsBySeller = cart.Items.GroupBy(item => productsDict[item.ProductId].SellerId);
        var ordersToCreate = new List<Core.Models.Order.Order>();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var group in itemsBySeller)
            {
                var sellerId = group.Key;
                var orderItems = group.Select(i => OrderItem.AddOrderItem(
                    i.ProductId,
                    productsDict[i.ProductId].Title,
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

                await _orderRepository.AddAsync(order, cancellationToken);
                ordersToCreate.Add(order);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return ordersToCreate.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<OrderDto>> GetBuyerOrdersAsync(Guid buyerId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        if (buyerId != currentUserId)
            throw new UnauthorizedException();

        var orders = await _orderRepository.GetByBuyerIdAsync(buyerId, cancellationToken);
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetSellerOrdersAsync(Guid sellerId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetBySellerIdAsync(sellerId, cancellationToken);
        return orders.Select(MapToDto);
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status, Guid currentUserId, bool isUserSeller, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            throw new OrderNotFoundException();

        if (!isUserSeller || order.SellerId != currentUserId)
            throw new UnauthorizedException();

        order.UpdateStatus(status);
        await _orderRepository.UpdateAsync(order, cancellationToken);
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
            Items = order.Items?.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                Title = i.Title,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList() ?? new List<OrderItemDto>()
        };
    }
}