using FluentValidation;
using Lab_07.DTOs;
using Lab_07.Services;

namespace Lab_07.Validators;

public class CreateOrderValidator : AbstractValidator<AddProductToWarehouseRequest>
{
    public CreateOrderValidator(IDbServiceDapper dbService)
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CreatedAt).NotEmpty();

        // we check if the product exists in the database
        RuleFor(x => x.ProductId).MustAsync(async (productId, cancellation) =>
        {
            var product = await dbService.GetProduct(productId);
            return product != null;
        }).WithMessage("Product does not exist");

        // we check if the warehouse exists in the database
        RuleFor(x => x.WarehouseId).MustAsync(async (warehouseId, cancellation) =>
        {
            var warehouse = await dbService.GetWarehouse(warehouseId);
            return warehouse != null;
        }).WithMessage("Warehouse does not exist");

        // we check if the product exists in Order table and has correct amount
        RuleFor(x => x).MustAsync(async (request, cancellation) =>
        {
            var order = await dbService.GetOrder(request.ProductId);
            return order != null && order.Amount == request.Amount && order.CreatedAt < request.CreatedAt;
        }).WithMessage("Order does not exist");

        // we check if the order is not fulfilled
        RuleFor(x => x).MustAsync(async (request, cancellation) =>
        {
            var isFulfilled = await dbService.IsOrderFulfilled(request.ProductId, request.Amount);
            return !isFulfilled;
        }).WithMessage("Order is already fulfilled");
    }
}