using FluentValidation;
using Lab_07.Endpoints;
using Lab_07.Services;
using Lab_07.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register custom services
builder.Services.AddScoped<IDbServiceDapper, DbServiceDapper>();

// register validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterWarehouseManagementEndpoints();

app.Run();
