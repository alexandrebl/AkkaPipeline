using Akka.Actor;
using AkkaTest.Actors;
using AkkaTest.Application.Interfaces;

namespace AkkaTest.Configuration;

/// <summary>
/// Factory Pattern: Cria e configura o pipeline de atores.
/// 
/// Princípios aplicados:
///   - Factory Pattern (GoF): Encapsula a criação de objetos complexos
///   - Dependency Injection: Injeta todas as dependências necessárias
///   - Single Responsibility: Responsável apenas pela criação do pipeline
/// 
/// Vantagens:
///   - Centraliza a configuração do sistema de atores
///   - Facilita testes unitários (pode criar com mocks)
///   - Reduz acoplamento entre Program e implementações
/// </summary>
public sealed class ActorSystemFactory
{
    private readonly ActorSystem _actorSystem;
    private readonly IOrderRepository _orderRepository;
    private readonly IValidationService _validationService;
    private readonly IPaymentService _paymentService;
    private readonly IShippingService _shippingService;
    private readonly IEmailNotificationService _emailService;
    private readonly ISmsNotificationService _smsService;

    public ActorSystemFactory(
        ActorSystem actorSystem,
        IOrderRepository orderRepository,
        IValidationService validationService,
        IPaymentService paymentService,
        IShippingService shippingService,
        IEmailNotificationService emailService,
        ISmsNotificationService smsService)
    {
        _actorSystem = actorSystem ?? throw new ArgumentNullException(nameof(actorSystem));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _shippingService = shippingService ?? throw new ArgumentNullException(nameof(shippingService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
    }

    /// <summary>
    /// Cria o pipeline de atores na ordem correta.
    /// Retorna o ator de entrada (OrderReceiverActor).
    /// </summary>
    public IActorRef CreateOrderProcessingPipeline()
    {
        // Cria os atores na ordem inversa (do fim para o início)
        // Cada ator recebe a referência do próximo ator no construtor

        // Ator 5: Notification (final do pipeline)
        var notificationActor = _actorSystem.ActorOf(
            NotificationActor.Create(_emailService, _smsService),
            "notification-actor"
        );

        // Ator 4: Shipping
        var shippingActor = _actorSystem.ActorOf(
            ShippingActor.Create(_shippingService, notificationActor),
            "shipping-actor"
        );

        // Ator 3: Payment
        var paymentActor = _actorSystem.ActorOf(
            PaymentActor.Create(_paymentService, shippingActor),
            "payment-actor"
        );

        // Ator 2: Validation
        var validationActor = _actorSystem.ActorOf(
            ValidationActor.Create(_validationService, paymentActor),
            "validation-actor"
        );

        // Ator 1: Order Receiver (início do pipeline)
        var orderReceiverActor = _actorSystem.ActorOf(
            OrderReceiverActor.Create(_orderRepository, validationActor),
            "order-receiver-actor"
        );

        return orderReceiverActor;
    }
}
