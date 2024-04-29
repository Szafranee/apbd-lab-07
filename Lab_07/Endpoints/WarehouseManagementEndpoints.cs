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
                AddProductToWarehouseRequest createOrderRequest,
                IValidator<AddProductToWarehouseRequest> newOrderValidator) =>
            {
                var validation = await newOrderValidator.ValidateAsync(createOrderRequest);
                if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

                if(await dbServiceDapper.GetProduct(createOrderRequest.ProductId) == null)
                {
                    return Results.NotFound("Product does not exist");
                }

                if(await dbServiceDapper.GetWarehouse(createOrderRequest.WarehouseId) == null)
                {
                    return Results.NotFound("Warehouse does not exist");
                }

                var order = await dbServiceDapper.GetOrder(createOrderRequest.ProductId);
                if (order == null || order.Amount != createOrderRequest.Amount ||
                    order.CreatedAt > createOrderRequest.CreatedAt)
                {
                    return Results.NotFound("Order does not exist");
                }

                if(await dbServiceDapper.IsOrderFulfilled(createOrderRequest.ProductId, createOrderRequest.Amount))
                {
                    return Results.NotFound("Order is already fulfilled");
                }

                var orderId = await dbServiceDapper.AddProductToWarehouse(createOrderRequest);
                return Results.Created("", orderId);
            });


        app.MapPost("/warehouse-proc", async (IDbServiceDapper dbServiceDapper,
             AddProductToWarehouseRequest addProductToWarehouseRequest, IValidator<AddProductToWarehouseRequest> validator) =>
        {
            var validation = await validator.ValidateAsync(addProductToWarehouseRequest);
            if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

            try
            {
                var orderId = await dbServiceDapper.AddProductToWarehouseProcedure(addProductToWarehouseRequest);
                return Results.Created("", orderId);
            }
            catch (SqlException e)
            {
                return Results.NotFound(e.Message);
            }

        });
    }
}