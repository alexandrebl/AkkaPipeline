using Akka.Actor;
using AkkaTest.Application.Interfaces;
using AkkaTest.Application.Messages;

namespace AkkaTest.Actors;

/// <summary>
/// ATOR 1: Order Receiver Actor
/// 
/// Princípios aplicados:
///   - Single Responsibility: Apenas recebe e persiste pedidos
///   - Dependency Inversion: Depende de IOrderRepository (abstração)
///   - Open/Closed: Extensível através da interface
/// </summary>
public sealed class OrderReceiverActor : ReceiveActor
{
    private readonly IOrderRepository _orderRepository;
    private readonly IActorRef _validationActor;

    public OrderReceiverActor(IOrderRepository orderRepository, IActorRef validationActor)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _validationActor = validationActor ?? throw new ArgumentNullException(nameof(validationActor));

        ReceiveAsync<ProcessOrderCommand>(HandleProcessOrderAsync);
    }

    private async Task HandleProcessOrderAsync(ProcessOrderCommand command)
    {
        var order = command.Order;
        
        Console.WriteLine($"[OrderReceiver] Pedido #{order.Id} recebido: {order.Product} x{order.Quantity}");

        try
        {
            // Usa o repositório para persistir (Dependency Injection)
            await _orderRepository.SaveAsync(order);
            Console.WriteLine($"[OrderReceiver] Pedido #{order.Id} salvo no banco de dados");

            // Encaminha para o próximo ator
            _validationActor.Tell(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrderReceiver] Erro ao processar pedido #{order.Id}: {ex.Message}");
            // Em produção: implementaria retry logic ou dead letter queue
        }
    }

    /// <summary>
    /// Factory Method Pattern: Cria Props com as dependências.
    /// </summary>
    public static Props Create(IOrderRepository orderRepository, IActorRef validationActor)
    {
        return Props.Create(() => new OrderReceiverActor(orderRepository, validationActor));
    }
}
