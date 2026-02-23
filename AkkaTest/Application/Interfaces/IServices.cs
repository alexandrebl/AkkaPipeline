using AkkaTest.Domain.Entities;

namespace AkkaTest.Application.Interfaces;

/// <summary>
/// Interface: Repository Pattern
/// Abstração para persistência de pedidos.
/// Segue o princípio de Inversão de Dependência (SOLID - D).
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Salva um pedido no repositório.
    /// </summary>
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um pedido por ID.
    /// </summary>
    Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface: Serviço de validação de pedidos.
/// Permite diferentes implementações de validação (Strategy Pattern).
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Valida um pedido de acordo com as regras de negócio.
    /// </summary>
    Task<(bool IsValid, string? Message)> ValidateOrderAsync(Order order, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface: Serviço de processamento de pagamentos.
/// Abstração que permite diferentes gateways de pagamento.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Processa o pagamento de um pedido.
    /// </summary>
    Task<string> ProcessPaymentAsync(int orderId, decimal amount, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface: Serviço de envio e logística.
/// Abstração para diferentes transportadoras.
/// </summary>
public interface IShippingService
{
    /// <summary>
    /// Cria uma etiqueta de envio e retorna o código de rastreamento.
    /// </summary>
    Task<string> CreateShippingLabelAsync(int orderId, string product, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface: Serviço de notificações por email.
/// Segrega a responsabilidade de envio de email (Interface Segregation - SOLID - I).
/// </summary>
public interface IEmailNotificationService
{
    /// <summary>
    /// Envia notificação por email.
    /// </summary>
    Task SendOrderShippedEmailAsync(int orderId, string product, string trackingNumber, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface: Serviço de notificações por SMS.
/// Separado do email para maior flexibilidade.
/// </summary>
public interface ISmsNotificationService
{
    /// <summary>
    /// Envia notificação por SMS.
    /// </summary>
    Task SendOrderShippedSmsAsync(int orderId, CancellationToken cancellationToken = default);
}
