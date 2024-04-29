namespace Lab_07.DTOs;

public record AddProductToWarehouseRequest(int ProductId, int WarehouseId, int Amount, DateTime CreatedAt);
