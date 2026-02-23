using Akka.Actor;
using AkkaTest.Application.Interfaces;
using AkkaTest.Application.Messages;

namespace AkkaTest.Actors;

/// <summary>
/// ATOR 5: Notification Actor
/// 
/// Princípios aplicados:
///   - Single Responsibility: Apenas envia notificações
///   - Dependency Inversion: Depende de interfaces de notificação
///   - Interface Segregation: Usa interfaces específicas (email e SMS separados)
/// </summary>
public sealed class NotificationActor : ReceiveActor
{
    private readonly IEmailNotificationService _emailService;
    private readonly ISmsNotificationService _smsService;

    public NotificationActor(
        IEmailNotificationService emailService,
        ISmsNotificationService smsService)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));

        ReceiveAsync<OrderShippedEvent>(HandleSendNotificationsAsync);
    }

    private async Task HandleSendNotificationsAsync(OrderShippedEvent shippedEvent)
    {
        var order = shippedEvent.Order;
        
        Console.WriteLine($"[Notification] Enviando notificação para pedido #{order.Id}...");

        try
        {
            // Envia notificações usando os serviços injetados
            // Executa em paralelo para otimizar tempo
            await Task.WhenAll(
                _emailService.SendOrderShippedEmailAsync(order.Id, order.Product, shippedEvent.TrackingNumber),
                _smsService.SendOrderShippedSmsAsync(order.Id)
            );

            // Exibe resumo final
            Console.WriteLine($"[Notification] ✓ Pedido #{order.Id} concluído com sucesso!");
            Console.WriteLine($"[Notification]   Produto: {order.Product}");
            Console.WriteLine($"[Notification]   Rastreio: {shippedEvent.TrackingNumber}");

            // Pipeline completo finalizado
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Notification] Erro ao enviar notificações do pedido #{order.Id}: {ex.Message}");
            // Em produção: implementaria retry ou fallback
        }
    }

    /// <summary>
    /// Factory Method Pattern: Cria Props com as dependências.
    /// </summary>
    public static Props Create(
        IEmailNotificationService emailService,
        ISmsNotificationService smsService)
    {
        return Props.Create(() => new NotificationActor(emailService, smsService));
    }
}
