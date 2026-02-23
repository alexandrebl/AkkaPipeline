# 🚀 Sistema de Processamento de Pedidos com Akka.NET

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![Akka.NET](https://img.shields.io/badge/Akka.NET-latest-blue)](https://getakka.net/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![SOLID](https://img.shields.io/badge/Principles-SOLID-orange)](https://en.wikipedia.org/wiki/SOLID)

Sistema de processamento de pedidos construído com **Akka.NET**, aplicando **Clean Architecture**, **SOLID Principles**, **Design Patterns GoF** e **Clean Code**.

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Começando](#-começando)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Princípios Aplicados](#-princípios-aplicados)
- [Como Funciona](#-como-funciona)
- [Testes](#-testes)
- [Contribuindo](#-contribuindo)

---

## 🎯 Sobre o Projeto

Este projeto demonstra a implementação de um **pipeline de processamento de pedidos** usando o framework **Akka.NET** para programação orientada a atores, com foco em:

- ✅ **Clean Architecture**: Separação de camadas (Domain, Application, Infrastructure)
- ✅ **SOLID Principles**: Todos os 5 princípios aplicados
- ✅ **Design Patterns**: Factory, Repository, Strategy, Chain of Responsibility
- ✅ **Async/Await**: Processamento assíncrono em todas as operações
- ✅ **Dependency Injection**: Inversão de dependências e injeção via construtor
- ✅ **Imutabilidade**: Uso de records para mensagens

---

## 🏗️ Arquitetura

O sistema implementa um **pipeline de 5 atores** que processam pedidos em etapas:

```
┌─────────────┐    ┌────────────┐    ┌──────────┐    ┌──────────┐    ┌──────────────┐
│   Order     │───▶│ Validation │───▶│ Payment  │───▶│ Shipping │───▶│ Notification │
│  Receiver   │    │   Actor    │    │  Actor   │    │  Actor   │    │    Actor     │
└─────────────┘    └────────────┘    └──────────┘    └──────────┘    └──────────────┘
     Etapa 1           Etapa 2          Etapa 3         Etapa 4          Etapa 5
```

### Fluxo de Processamento:

1. **OrderReceiverActor**: Recebe e persiste pedidos
2. **ValidationActor**: Valida dados e regras de negócio
3. **PaymentActor**: Processa pagamento via gateway
4. **ShippingActor**: Cria etiqueta de envio e código de rastreio
5. **NotificationActor**: Envia notificações (email/SMS)

📖 **[Ver Documentação Completa da Arquitetura](./ARCHITECTURE.md)**

---

## 🛠️ Tecnologias

- **.NET 10** - Framework de desenvolvimento
- **C# 14** - Linguagem de programação
- **Akka.NET** - Framework de atores para sistemas concorrentes
- **Records** - Value Objects imutáveis
- **Async/Await** - Programação assíncrona

---

## 🚀 Começando

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- IDE: Visual Studio 2026+ ou Visual Studio Code

### Instalação

1. **Clone o repositório**
```bash
git clone https://github.com/seu-usuario/akka-test.git
cd akka-test
```

2. **Restaure as dependências**
```bash
dotnet restore
```

3. **Execute o projeto**
```bash
dotnet run --project AkkaTest
```

### Saída Esperada

```
=== Sistema de Processamento de Pedidos - Iniciado ===

Pedidos enviados para processamento...

[OrderReceiver] Pedido #1 recebido: Notebook x1
[OrderReceiver] Pedido #1 salvo no banco de dados
[Validation] Validando pedido #1...
[Validation] Pedido #1 validado com sucesso!
[Payment] Processando pagamento de R$ 3500.00 para pedido #1...
[Payment] Pagamento aprovado! Transaction ID: A7B3C2D1
[Shipping] Preparando envio do pedido #1...
[Shipping] Pedido #1 despachado! Código de rastreio: BR12345678
[Notification] Enviando notificação para pedido #1...
[Notification] ✓ Pedido #1 concluído com sucesso!
[Notification]   Produto: Notebook
[Notification]   Rastreio: BR12345678
```

---

## 📂 Estrutura do Projeto

```
AkkaTest/
│
├── Domain/                          # Camada de Domínio
│   └── Entities/
│       └── Order.cs                 # Entidade de negócio
│
├── Application/                     # Camada de Aplicação
│   ├── Interfaces/
│   │   └── IServices.cs            # Contratos (abstrações)
│   └── Messages/
│       └── OrderMessages.cs        # Value Objects (mensagens)
│
├── Infrastructure/                  # Camada de Infraestrutura
│   ├── Persistence/
│   │   └── InMemoryOrderRepository.cs
│   └── Services/
│       └── ExternalServices.cs     # Implementações
│
├── Actors/                          # Camada de Atores
│   ├── OrderReceiverActor.cs
│   ├── ValidationActor.cs
│   ├── PaymentActor.cs
│   ├── ShippingActor.cs
│   └── NotificationActor.cs
│
├── Configuration/
│   └── ActorSystemFactory.cs       # Factory Pattern
│
├── Program.cs                       # Entry Point
├── ARCHITECTURE.md                  # Documentação de Arquitetura
└── README.md                        # Este arquivo
```

---

## ⚙️ Princípios Aplicados

### SOLID

| Princípio | Aplicação |
|-----------|-----------|
| **S**ingle Responsibility | Cada ator tem uma única responsabilidade |
| **O**pen/Closed | Extensível através de interfaces |
| **L**iskov Substitution | Interfaces substituíveis |
| **I**nterface Segregation | Interfaces específicas (`IEmailService`, `ISmsService`) |
| **D**ependency Inversion | Depende de abstrações, não implementações |

### Design Patterns (GoF)

- ✅ **Factory Pattern**: `ActorSystemFactory`
- ✅ **Repository Pattern**: `IOrderRepository`
- ✅ **Strategy Pattern**: `IPaymentService`, `IShippingService`
- ✅ **Chain of Responsibility**: Pipeline de atores
- ✅ **Dependency Injection**: Injeção via construtor

---

## 📖 Como Funciona

### 1. Criação do Pipeline

```csharp
var actorFactory = new ActorSystemFactory(
    actorSystem,
    orderRepository,
    validationService,
    paymentService,
    shippingService,
    emailService,
    smsService
);

var orderReceiver = actorFactory.CreateOrderProcessingPipeline();
```

### 2. Envio de Pedidos

```csharp
var order = new Order 
{ 
    Id = 1, 
    Product = "Notebook", 
    Quantity = 1, 
    Price = 3500.00m 
};

orderReceiver.Tell(new ProcessOrderCommand(order));
```

### 3. Processamento Assíncrono

Cada ator processa a mensagem de forma assíncrona e encaminha para o próximo ator na cadeia:

```csharp
public class PaymentActor : ReceiveActor
{
    private readonly IPaymentService _paymentService;
    private readonly IActorRef _nextActor;

    public PaymentActor(IPaymentService paymentService, IActorRef nextActor)
    {
        _paymentService = paymentService;
        _nextActor = nextActor;

        ReceiveAsync<OrderValidatedEvent>(async evt =>
        {
            var transactionId = await _paymentService.ProcessPaymentAsync(...);
            _nextActor.Tell(new OrderPaidEvent(..., transactionId));
        });
    }
}
```

---

## 🧪 Testes

### Estrutura de Testes (Proposta)

```
AkkaTest.Tests/
├── Unit/
│   ├── Actors/
│   │   ├── OrderReceiverActorTests.cs
│   │   ├── ValidationActorTests.cs
│   │   └── PaymentActorTests.cs
│   └── Services/
│       ├── OrderValidationServiceTests.cs
│       └── PaymentGatewayServiceTests.cs
└── Integration/
    └── OrderProcessingPipelineTests.cs
```

### Exemplo de Teste Unitário

```csharp
[Fact]
public async Task PaymentActor_ShouldProcessPayment_WhenOrderIsValid()
{
    // Arrange
    var mockPaymentService = new Mock<IPaymentService>();
    mockPaymentService
        .Setup(x => x.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<decimal>()))
        .ReturnsAsync("TXN-12345");

    var probe = TestActor;
    var sut = ActorOf(() => new PaymentActor(mockPaymentService.Object, probe));

    // Act
    sut.Tell(new OrderValidatedEvent(order, true));

    // Assert
    await ExpectMsgAsync<OrderPaidEvent>();
    mockPaymentService.Verify(x => x.ProcessPaymentAsync(1, 3500m), Times.Once);
}
```

---

## 🔧 Configuração para Produção

### Usando Microsoft.Extensions.DependencyInjection

```csharp
var services = new ServiceCollection();

// Registra serviços
services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
services.AddSingleton<IValidationService, OrderValidationService>();
services.AddSingleton<IPaymentService, PaymentGatewayService>();
// ...

// Registra Akka.NET
services.AddSingleton<ActorSystem>(sp => ActorSystem.Create("OrderSystem"));
services.AddSingleton<ActorSystemFactory>();

var provider = services.BuildServiceProvider();
var factory = provider.GetRequiredService<ActorSystemFactory>();
var pipeline = factory.CreateOrderProcessingPipeline();
```

---

## 📈 Melhorias Futuras

- [ ] Adicionar Akka.Persistence para estado durável
- [ ] Implementar Circuit Breaker (Polly)
- [ ] Adicionar logging estruturado (Serilog)
- [ ] Implementar métricas e monitoring (Prometheus)
- [ ] Adicionar Health Checks
- [ ] Implementar Dead Letter Queue
- [ ] Adicionar Saga Pattern para compensação
- [ ] Distribuir atores em cluster (Akka.Cluster)

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para:

1. Fazer um fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abrir um Pull Request

---

## 📝 Licença

Este projeto é de código aberto e está disponível sob a [MIT License](LICENSE).

---

## 👨‍💻 Autor

**Alexandre**

- GitHub: [@seu-usuario](https://github.com/seu-usuario)
- LinkedIn: [seu-perfil](https://linkedin.com/in/seu-perfil)

---

## 🙏 Agradecimentos

- [Akka.NET Team](https://getakka.net/)
- [Uncle Bob](https://blog.cleancoder.com/) - Clean Architecture
- [Gang of Four](https://en.wikipedia.org/wiki/Design_Patterns) - Design Patterns

---

⭐ Se este projeto foi útil para você, considere dar uma estrela!
