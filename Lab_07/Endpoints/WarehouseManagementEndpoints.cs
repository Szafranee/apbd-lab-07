using System.Data.SqlClient;
using FluentValidation;
using Lab_07.DTOs;
using Lab_07.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab_07.Endpoints;

public static class WarehouseManagementEndpoints
{
    public static void RegisterWarehouseManagementEndpoints(this WebApplication app)
    {
        //POST /warehouses
        app.MapPost("/warehouses",
            async (IConfiguration configuration, IDbServiceDapper dbServiceDapper,
                [FromBody] CreateOrderRequest createOrderRequest, IValidator<CreateOrderRequest> newOrderValidator) =>
            {
                var validationResult = await newOrderValidator.ValidateAsync(createOrderRequest);
                if (!validationResult.IsValid) return Results.ValidationProblem(validationResult.ToDictionary());

                var orderId = await dbServiceDapper.CreateOrder(createOrderRequest);
                return Results.Created("", orderId);
            });
    }
}