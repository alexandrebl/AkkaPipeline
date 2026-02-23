namespace AkkaTest.Domain.Entities;

/// <summary>
/// Domain Entity: Order
/// Representa um pedido no domínio da aplicação.
/// Segue os princípios de DDD (Domain-Driven Design).
/// </summary>
public sealed class Order
{
    public int Id { get; init; }
    public string Product { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }

    /// <summary>
    /// Calcula o valor total do pedido.
    /// Encapsula a lógica de negócio dentro da entidade.
    /// </summary>
    public decimal TotalAmount => Price * Quantity;

    /// <summary>
    /// Valida se o pedido possui dados básicos corretos.
    /// Regras de negócio do domínio.
    /// </summary>
    public bool IsValid() => Quantity > 0 && Price > 0 && !string.IsNullOrWhiteSpace(Product);
}
