namespace Lab_07.DTOs;

public record CreateOrderRequest(int ProductId, int WarehouseId, int Amount, DateTime CreatedAt);

public record GetOrderResponseFromWarehouse(int Id, int WarehouseId, int ProductId, int OrderId, int Amount, decimal Price, DateTime CreatedAt);