// ========================================================================================================
// SISTEMA DE PROCESSAMENTO DE PEDIDOS COM AKKA.NET
// REFATORADO COM CLEAN ARCHITECTURE, SOLID E DESIGN PATTERNS
// ========================================================================================================
//
// CLEAN ARCHITECTURE - ESTRUTURA DE CAMADAS:
// --------------------------------------------------------------------------------------------------------
// Domain/           → Entidades e regras de negócio do domínio
// Application/      → Interfaces, DTOs e contratos de aplicação
// Infrastructure/   → Implementações de serviços externos (DB, APIs)
// Actors/           → Atores Akka.NET desacoplados usando DI
// Configuration/    → Factories e configuração do sistema
//
// --------------------------------------------------------------------------------------------------------
// PRINCÍPIOS SOLID APLICADOS:
// --------------------------------------------------------------------------------------------------------
// [S] Single Responsibility   → Cada classe tem uma única responsabilidade
// [O] Open/Closed             → Extensível através de interfaces
// [L] Liskov Substitution     → Interfaces podem ser substituídas por implementações
// [I] Interface Segregation   → Interfaces específicas (IEmailService, ISmsService separados)
// [D] Dependency Inversion    → Depende de abstrações, não de implementações concretas
//
// --------------------------------------------------------------------------------------------------------
// DESIGN PATTERNS (GoF) IMPLEMENTADOS:
// --------------------------------------------------------------------------------------------------------
// - Factory Pattern           → ActorSystemFactory cria o pipeline de atores
// - Repository Pattern        → IOrderRepository para acesso a dados
// - Strategy Pattern          → Permite trocar implementações (payment, shipping, validation)
// - Chain of Responsibility   → Pipeline de atores (inerente ao design)
// - Dependency Injection      → Todas as dependências são injetadas
//
// --------------------------------------------------------------------------------------------------------

using Akka.Actor;
using AkkaTest.Application.Interfaces;
using AkkaTest.Application.Messages;
using AkkaTest.Configuration;
using AkkaTest.Domain.Entities;
using AkkaTest.Infrastructure.Persistence;
using AkkaTest.Infrastructure.Services;

// ========================================================================================================
// CONFIGURAÇÃO DE DEPENDÊNCIAS (Composition Root)
// ========================================================================================================
// Aqui configuramos todas as dependências do sistema.
// Em uma aplicação real, usaríamos um container de DI (Microsoft.Extensions.DependencyInjection)
// ========================================================================================================

// Cria o ActorSystem
var actorSystem = ActorSystem.Create("OrderProcessingSystem");

// Instancia os serviços (Infrastructure Layer)
IOrderRepository orderRepository = new InMemoryOrderRepository();
IValidationService validationService = new OrderValidationService();
IPaymentService paymentService = new PaymentGatewayService();
IShippingService shippingService = new ShippingCarrierService();
IEmailNotificationService emailService = new EmailNotificationService();
ISmsNotificationService smsService = new SmsNotificationService();

// Cria a factory que montará o pipeline de atores
var actorFactory = new ActorSystemFactory(
    actorSystem,
    orderRepository,
    validationService,
    paymentService,
    shippingService,
    emailService,
    smsService
);

// ========================================================================================================
// CRIAÇÃO DO PIPELINE DE ATORES
// ========================================================================================================
// Factory Pattern: Delega a criação do pipeline para ActorSystemFactory
// ========================================================================================================

var orderReceiverActor = actorFactory.CreateOrderProcessingPipeline();

// ========================================================================================================
// ENVIO DE PEDIDOS PARA PROCESSAMENTO
// ========================================================================================================
// Usa Value Objects imutáveis (records) como mensagens
// ========================================================================================================

Console.WriteLine("=== Sistema de Processamento de Pedidos - Iniciado ===\n");

// Cria pedidos usando a entidade de domínio
var order1 = new Order { Id = 1, Product = "Notebook", Quantity = 1, Price = 3500.00m };
var order2 = new Order { Id = 2, Product = "Mouse", Quantity = 2, Price = 50.00m };
var order3 = new Order { Id = 3, Product = "Teclado Mecânico", Quantity = 1, Price = 450.00m };

// Envia comandos para o pipeline
orderReceiverActor.Tell(new ProcessOrderCommand(order1));
orderReceiverActor.Tell(new ProcessOrderCommand(order2));
orderReceiverActor.Tell(new ProcessOrderCommand(order3));

Console.WriteLine("Pedidos enviados para processamento...\n");

// ========================================================================================================
// MONITORAMENTO ASSÍNCRONO DO SISTEMA
// ========================================================================================================

var cancellationTokenSource = new CancellationTokenSource();
var monitoringTask = MonitorSystemAsync(cancellationTokenSource.Token);

// Aguarda tempo suficiente para processar os pedidos
await Task.Delay(TimeSpan.FromSeconds(10));

// Cancela o monitoramento
cancellationTokenSource.Cancel();

try
{
    await monitoringTask;
}
catch (OperationCanceledException)
{
    // Esperado ao cancelar
}

// ========================================================================================================
// ENCERRAMENTO GRACIOSO DO SISTEMA
// ========================================================================================================

Console.WriteLine("\n=== Encerrando o sistema de atores ===");
await actorSystem.Terminate();
Console.WriteLine("Sistema encerrado com sucesso!");

// ========================================================================================================
// MÉTODO DE MONITORAMENTO
// ========================================================================================================

static async Task MonitorSystemAsync(CancellationToken cancellationToken)
{
    int tick = 0;

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            tick++;
            Console.WriteLine($"\n[Monitor] Tick #{tick} - Sistema operacional");
        }
        catch (OperationCanceledException)
        {
            break;
        }
    }
}
