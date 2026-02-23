using Akka.Actor;
using AkkaTest.Application.Interfaces;
using AkkaTest.Application.Messages;

namespace AkkaTest.Actors;

/// <summary>
/// ATOR 3: Payment Actor
/// 
/// Princípios aplicados:
///   - Single Responsibility: Apenas processa pagamentos
///   - Dependency Inversion: Depende de IPaymentService (abstração)
///   - Strategy Pattern: Permite trocar gateway de pagamento
/// </summary>
public sealed class PaymentActor : ReceiveActor
{
    private readonly IPaymentService _paymentService;
    private readonly IActorRef _shippingActor;

    public PaymentActor(IPaymentService paymentService, IActorRef shippingActor)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _shippingActor = shippingActor ?? throw new ArgumentNullException(nameof(shippingActor));

        ReceiveAsync<OrderValidatedEvent>(HandleProcessPaymentAsync);
    }

    private async Task HandleProcessPaymentAsync(OrderValidatedEvent validatedEvent)
    {
        if (!validatedEvent.IsValid)
            return;

        var order = validatedEvent.Order;
        
        Console.WriteLine($"[Payment] Processando pagamento de R$ {order.TotalAmount:F2} para pedido #{order.Id}...");

        try
        {
            // Usa o serviço de pagamento (Dependency Injection)
            string transactionId = await _paymentService.ProcessPaymentAsync(order.Id, order.TotalAmount);
            
            Console.WriteLine($"[Payment] Pagamento aprovado! Transaction ID: {transactionId}");

            // Cria evento de pagamento e encaminha
            var paidEvent = new OrderPaidEvent(order, transactionId, order.TotalAmount);
            _shippingActor.Tell(paidEvent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Payment] Erro ao processar pagamento do pedido #{order.Id}: {ex.Message}");
            // Em produção: implementaria compensação de transação
        }
    }

    /// <summary>
    /// Factory Method Pattern: Cria Props com as dependências.
    /// </summary>
    public static Props Create(IPaymentService paymentService, IActorRef shippingActor)
    {
        return Props.Create(() => new PaymentActor(paymentService, shippingActor));
    }
}
