using CryptoExchangeTask.Core;
using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IExchangeRepository, ExchangeRepository>();
builder.Services.AddTransient<BuyOrderPlanner>();
builder.Services.AddTransient<SellOrderPlanner>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGet("/", () => Results.Redirect("/openapi/v1.json"));
}

//app.UseHttpsRedirection();

app.MapGet("/plan/buy", (decimal amount, BuyOrderPlanner planner) => ExecutePlan(amount, planner));

app.MapGet("/plan/sell", (decimal amount, SellOrderPlanner planner) => ExecutePlan(amount, planner));

app.Run();

static IResult ExecutePlan(decimal amount, OrderPlanner planner)
{
    try
    {
        return Results.Ok(planner.Plan(amount));
    }
    catch (InsufficientBalanceException)
    {
        return Results.BadRequest("You do not have enough funds or there are not enough available orders to complete this action");
    }
}