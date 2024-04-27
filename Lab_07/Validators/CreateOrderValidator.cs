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
    }
}