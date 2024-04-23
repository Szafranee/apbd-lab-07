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
            async (IDbServiceDapper dbServiceDapper,
                [FromBody] AddProductToWarehouseRequest createOrderRequest,
                IValidator<AddProductToWarehouseRequest> newOrderValidator) =>
            {
                var validationResult = await newOrderValidator.ValidateAsync(createOrderRequest);
                if (!validationResult.IsValid) return Results.ValidationProblem(validationResult.ToDictionary());

                var orderId = await dbServiceDapper.AddProductToWarehouse(createOrderRequest);
                return Results.Created("", orderId);
            });


        app.MapPost("/warehouse-proc", async (IDbServiceDapper dbServiceDapper,
            [FromBody] AddProductToWarehouseRequest addProductToWarehouseRequest) =>
        {
            var orderId = await dbServiceDapper.AddProductToWarehouseProcedure(addProductToWarehouseRequest);
            return Results.Created("", orderId);
        });
    }
}