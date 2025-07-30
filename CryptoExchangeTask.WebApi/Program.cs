using CryptoExchangeTask.Core;
using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IExchangeRepository, FileExchangeRepository>();
builder.Services.AddTransient<BuyOrderPlanner>();
builder.Services.AddTransient<SellOrderPlanner>();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapGet("/", () => TypedResults.Redirect("/openapi/v1.json")).ExcludeFromDescription();

// Disabled this, so that if there are certificate problems during demonstration it will still be possible to access the API
//app.UseHttpsRedirection();

app.MapGet("/plan/buy", (decimal amount, BuyOrderPlanner planner) => GeneratePlan(amount, planner));

app.MapGet("/plan/sell", (decimal amount, SellOrderPlanner planner) => GeneratePlan(amount, planner));

app.Run();

static Results<Ok<OrderPlan>, BadRequest<string>> GeneratePlan(decimal amount, OrderPlanner planner)
{
    if (amount <= 0)
    {
        return TypedResults.BadRequest("The amount must be greater than 0");
    }

    try
    {
        return TypedResults.Ok(planner.Plan(amount));
    }
    catch (InsufficientBalanceException)
    {
        return TypedResults.BadRequest(
            "You do not have enough funds or there are not enough available orders to complete this action");
    }
}