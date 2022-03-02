using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Carts.ShoppingCarts.ConfirmingCart;
using Carts.ShoppingCarts.Products;
using Core.Events;
using Core.Exceptions;
using Marten;

namespace Carts.ShoppingCarts.FinalizingCart;

public class ShoppingCartFinalized: IExternalEvent
{
    public Guid CartId { get; }

    public Guid ClientId { get; }

    public IReadOnlyList<PricedProductItem> ProductItems { get; }

    public decimal TotalPrice { get; }

    public DateTime FinalizedAt { get; }

    private ShoppingCartFinalized(
        Guid cartId,
        Guid clientId,
        IReadOnlyList<PricedProductItem> productItems,
        decimal totalPrice,
        DateTime finalizedAt)
    {
        CartId = cartId;
        ClientId = clientId;
        ProductItems = productItems;
        TotalPrice = totalPrice;
        FinalizedAt = finalizedAt;
    }

    public static ShoppingCartFinalized Create(
        Guid cartId,
        Guid clientId,
        IReadOnlyList<PricedProductItem> productItems,
        decimal totalPrice,
        DateTime finalizedAt)
    {
        return new(cartId, clientId, productItems, totalPrice, finalizedAt);
    }
}

internal class HandleCartFinalized : IEventHandler<ShoppingCartConfirmed>
{
    private readonly IQuerySession querySession;
    private readonly IEventBus eventBus;

    public HandleCartFinalized(
        IQuerySession querySession,
        IEventBus eventBus
    )
    {
        this.querySession = querySession;
        this.eventBus = eventBus;
    }

    public async Task Handle(ShoppingCartConfirmed @event, CancellationToken cancellationToken)
    {
        var cart = await querySession.LoadAsync<ShoppingCart>(@event.CartId, cancellationToken)
                   ?? throw  AggregateNotFoundException.For<ShoppingCart>(@event.CartId);

        var externalEvent = ShoppingCartFinalized.Create(
            @event.CartId,
            cart.ClientId,
            cart.ProductItems.ToList(),
            cart.TotalPrice,
            @event.ConfirmedAt
        );

        await eventBus.Publish(externalEvent, cancellationToken);
    }
}
