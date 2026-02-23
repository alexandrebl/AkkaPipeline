using AkkaTest.Application.Interfaces;
using AkkaTest.Domain.Entities;

namespace AkkaTest.Infrastructure.Persistence;

/// <summary>
/// Implementação concreta do Repository Pattern.
/// Simula operações de banco de dados.
/// Em produção: integraria com Entity Framework Core, Dapper, etc.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _orders = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Simula latência de I/O do banco de dados
            await Task.Delay(100, cancellationToken);

            _orders[order.Id] = order;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Simula latência de leitura
            await Task.Delay(50, cancellationToken);

            return _orders.TryGetValue(orderId, out var order) ? order : null;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
