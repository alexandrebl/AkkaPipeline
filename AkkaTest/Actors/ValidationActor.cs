using Akka.Actor;
using AkkaTest.Application.Interfaces;
using AkkaTest.Application.Messages;

namespace AkkaTest.Actors;

/// <summary>
/// ATOR 2: Validation Actor
/// 
/// Princípios aplicados:
///   - Single Responsibility: Apenas valida pedidos
///   - Dependency Inversion: Depende de IValidationService (abstração)
///   - Strategy Pattern: Pode usar diferentes estratégias de validação
/// </summary>
public sealed class ValidationActor : ReceiveActor
{
    private readonly IValidationService _validationService;
    private readonly IActorRef _paymentActor;

    public ValidationActor(IValidationService validationService, IActorRef paymentActor)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _paymentActor = paymentActor ?? throw new ArgumentNullException(nameof(paymentActor));

        ReceiveAsync<ProcessOrderCommand>(HandleValidateOrderAsync);
    }

    private async Task HandleValidateOrderAsync(ProcessOrderCommand command)
    {
        var order = command.Order;
        
        Console.WriteLine($"[Validation] Validando pedido #{order.Id}...");

        try
        {
            // Usa o serviço de validação (Dependency Injection)
            var (isValid, message) = await _validationService.ValidateOrderAsync(order);

            if (isValid)
            {
                Console.WriteLine($"[Validation] Pedido #{order.Id} validado com sucesso!");
                
                // Cria evento de validação e encaminha
                var validatedEvent = new OrderValidatedEvent(order, true, message);
                _paymentActor.Tell(validatedEvent);
            }
            else
            {
                Console.WriteLine($"[Validation] Pedido #{order.Id} inválido: {message}");
                // Pipeline interrompido - pedido não avança
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Validation] Erro ao validar pedido #{order.Id}: {ex.Message}");
        }
    }

    /// <summary>
    /// Factory Method Pattern: Cria Props com as dependências.
    /// </summary>
    public static Props Create(IValidationService validationService, IActorRef paymentActor)
    {
        return Props.Create(() => new ValidationActor(validationService, paymentActor));
    }
}
