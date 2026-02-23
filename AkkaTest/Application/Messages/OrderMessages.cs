using AkkaTest.Domain.Entities;

namespace AkkaTest.Application.Messages;

/// <summary>
/// Value Objects: Mensagens imutáveis do pipeline.
/// Cada mensagem é um Value Object que transporta dados entre atores.
/// Imutabilidade garante thread-safety no modelo de atores.
/// </summary>

/// <summary>
/// Comando inicial para processar um pedido.
/// </summary>
public sealed record ProcessOrderCommand(Order Order);

/// <summary>
/// Evento indicando que um pedido foi validado.
/// </summary>
public sealed record OrderValidatedEvent(Order Order, bool IsValid, string? ValidationMessage = null);

/// <summary>
/// Evento indicando que um pedido foi pago.
/// </summary>
public sealed record OrderPaidEvent(Order Order, string TransactionId, decimal AmountPaid);

/// <summary>
/// Evento indicando que um pedido foi enviado.
/// </summary>
public sealed record OrderShippedEvent(Order Order, string TrackingNumber);

/// <summary>
/// Evento final indicando que todas as notificações foram enviadas.
/// </summary>
public sealed record OrderCompletedEvent(Order Order, string Status);
