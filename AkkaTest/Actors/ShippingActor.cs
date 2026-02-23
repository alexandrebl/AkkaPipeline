using Akka.Actor;
using AkkaTest.Application.Interfaces;
using AkkaTest.Application.Messages;

namespace AkkaTest.Actors;

/// <summary>
/// ATOR 4: Shipping Actor
/// 
/// Princípios aplicados:
///   - Single Responsibility: Apenas processa envios
///   - Dependency Inversion: Depende de IShippingService (abstração)
///   - Strategy Pattern: Permite trocar transportadora
/// </summary>
public sealed class ShippingActor : ReceiveActor
{
    private readonly IShippingService _shippingService;
    private readonly IActorRef _notificationActor;

    public ShippingActor(IShippingService shippingService, IActorRef notificationActor)
    {
        _shippingService = shippingService ?? throw new ArgumentNullException(nameof(shippingService));
        _notificationActor = notificationActor ?? throw new ArgumentNullException(nameof(notificationActor));

        ReceiveAsync<OrderPaidEvent>(HandleCreateShipmentAsync);
    }

    private async Task HandleCreateShipmentAsync(OrderPaidEvent paidEvent)
    {
        var order = paidEvent.Order;
        
        Console.WriteLine($"[Shipping] Preparando envio do pedido #{order.Id}...");

        try
        {
            // Usa o serviço de envio (Dependency Injection)
            string trackingNumber = await _shippingService.CreateShippingLabelAsync(order.Id, order.Product);
            
            Console.WriteLine($"[Shipping] Pedido #{order.Id} despachado! Código de rastreio: {trackingNumber}");

            // Cria evento de envio e encaminha
            var shippedEvent = new OrderShippedEvent(order, trackingNumber);
            _notificationActor.Tell(shippedEvent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Shipping] Erro ao processar envio do pedido #{order.Id}: {ex.Message}");
        }
    }

    /// <summary>
    /// Factory Method Pattern: Cria Props com as dependências.
    /// </summary>
    public static Props Create(IShippingService shippingService, IActorRef notificationActor)
    {
        return Props.Create(() => new ShippingActor(shippingService, notificationActor));
    }
}
