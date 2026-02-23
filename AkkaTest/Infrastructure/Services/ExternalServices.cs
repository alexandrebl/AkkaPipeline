using AkkaTest.Application.Interfaces;
using AkkaTest.Domain.Entities;

namespace AkkaTest.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de validação.
/// Strategy Pattern: Pode ser substituído por outras estratégias de validação.
/// </summary>
public sealed class OrderValidationService : IValidationService
{
    public async Task<(bool IsValid, string? Message)> ValidateOrderAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        // Simula chamada a serviço externo de validação
        await Task.Delay(200, cancellationToken);

        // Aplica regras de negócio
        if (!order.IsValid())
        {
            return (false, "Pedido contém dados inválidos (quantidade ou preço zero/negativo)");
        }

        // Em produção: validaria estoque, limites de crédito, etc.
        // await _stockService.CheckAvailabilityAsync(order.Product, order.Quantity);

        return (true, "Pedido validado com sucesso");
    }
}

/// <summary>
/// Implementação do serviço de pagamento.
/// Strategy Pattern: Permite trocar gateway de pagamento facilmente.
/// </summary>
public sealed class PaymentGatewayService : IPaymentService
{
    public async Task<string> ProcessPaymentAsync(
        int orderId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        // Simula latência de chamada à API de pagamento (Stripe, PayPal, etc.)
        await Task.Delay(350, cancellationToken);

        // Em produção:
        // var result = await _stripeClient.Charges.CreateAsync(chargeOptions, cancellationToken: cancellationToken);
        // return result.Id;

        // Gera ID de transação simulado
        return Guid.NewGuid().ToString()[..8].ToUpperInvariant();
    }
}

/// <summary>
/// Implementação do serviço de envio.
/// Strategy Pattern: Permite trocar transportadora facilmente.
/// </summary>
public sealed class ShippingCarrierService : IShippingService
{
    public async Task<string> CreateShippingLabelAsync(
        int orderId,
        string product,
        CancellationToken cancellationToken = default)
    {
        // Simula latência de chamada à API de transportadora
        await Task.Delay(250, cancellationToken);

        // Em produção:
        // var label = await _correiosClient.CreateLabelAsync(order, cancellationToken);
        // return label.TrackingNumber;

        // Gera código de rastreamento simulado
        return $"BR{Random.Shared.Next(10000000, 99999999)}";
    }
}

/// <summary>
/// Implementação do serviço de email.
/// Single Responsibility: Responsável apenas por enviar emails.
/// </summary>
public sealed class EmailNotificationService : IEmailNotificationService
{
    public async Task SendOrderShippedEmailAsync(
        int orderId,
        string product,
        string trackingNumber,
        CancellationToken cancellationToken = default)
    {
        // Simula latência de chamada ao serviço de email
        await Task.Delay(150, cancellationToken);

        // Em produção:
        // var message = new EmailMessage
        // {
        //     To = customer.Email,
        //     Subject = $"Pedido #{orderId} enviado!",
        //     Body = $"Seu produto {product} foi enviado. Rastreie com: {trackingNumber}"
        // };
        // await _sendGridClient.SendEmailAsync(message, cancellationToken);
    }
}

/// <summary>
/// Implementação do serviço de SMS.
/// Single Responsibility: Responsável apenas por enviar SMS.
/// </summary>
public sealed class SmsNotificationService : ISmsNotificationService
{
    public async Task SendOrderShippedSmsAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        // Simula latência de chamada ao serviço de SMS
        await Task.Delay(100, cancellationToken);

        // Em produção:
        // await _twilioClient.Messages.CreateAsync(
        //     to: customer.Phone,
        //     body: $"Pedido #{orderId} enviado! Verifique seu email.",
        //     cancellationToken: cancellationToken
        // );
    }
}
