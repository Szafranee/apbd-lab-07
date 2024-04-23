using System.Data;
using System.Data.SqlClient;
using Dapper;
using Lab_07.DTOs;
using Lab_07.Models;

namespace Lab_07.Services;

public interface IDbServiceDapper
{
    Task<int> CreateOrder(CreateOrderRequest request);
    Task<Product?> GetProduct(int id);
    Task<Warehouse?> GetWarehouse(int id);
    Task<Order?> GetOrder(int id);
    Task<bool> IsOrderFulfilled(int productId, int amount);
}

public class DbServiceDapper(IConfiguration configuration) : IDbServiceDapper
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    public async Task<int> CreateOrder(CreateOrderRequest request)
    {
        await using var connection = await GetConnection();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var orderId = await GetOrderId(request.ProductId, request.Amount);
            Console.WriteLine(orderId);

            await connection.ExecuteAsync(
                "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IdWare, @IdProd, @IdOrd, @Am, @Pr, @CrAt)",
                new
                {
                    IdWare = request.WarehouseId,
                    IdProd = request.ProductId,
                    IdOrd = orderId,
                    Am = request.Amount,
                    Pr = GetProduct(request.ProductId).Result?.Price * request.Amount,
                    CrAt = request.CreatedAt
                },
                transaction);

            await connection.ExecuteAsync(
                "UPDATE [Order] SET FulfilledAt = @FulAt WHERE IdOrder = @Id",
                new { FulAt = request.CreatedAt, Id = orderId },
                transaction);

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdOrder = @Id",
                new { Id = orderId },
                transaction);

            await transaction.CommitAsync();

            // return the id of the just inserted record
            return result;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Product?> GetProduct(int id)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<Product>(
                "SELECT * FROM Product WHERE IdProduct = @Id",
                new { Id = id });

        return result;
    }

    public async Task<Warehouse?> GetWarehouse(int id)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<Warehouse>(
                "SELECT * FROM Warehouse WHERE IdWarehouse = @Id",
                new { Id = id });

        return result;
    }

    public async Task<Order?> GetOrder(int id)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<Order>(
                "SELECT * FROM [Order] WHERE IdProduct = @Id",
                new { Id = id });

        return result;
    }

    public async Task<bool> IsOrderFulfilled(int productId, int amount)
    {
        await using var connection = await GetConnection();

        var orderId = await GetOrderId(productId, amount);
        Console.WriteLine(orderId);

        var result =
            await connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT * FROM Product_Warehouse WHERE IdOrder = @Id",
                new { Id =  orderId});

        return result != null;
    }

    private async Task<int?> GetOrderId(int productId, int amount)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount",
                new { IdProduct = productId, Amount = amount});

        return result;
    }
}