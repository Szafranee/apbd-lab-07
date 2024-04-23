namespace Lab_07.DTOs;

public record AddProductToWarehouseRequest(int ProductId, int WarehouseId, int Amount, DateTime CreatedAt);

public record GetOrderResponseFromWarehouse(int Id, int WarehouseId, int ProductId, int OrderId, int Amount, decimal Price, DateTime CreatedAt);