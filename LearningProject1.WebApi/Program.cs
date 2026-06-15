using LearningProject1.WebApi.Middleware;
using LearningProject1.Core.Interfaces;
using LearningProject1.Core.Exceptions;
using LearningProject1.Core.Services;
using LearningProject1.DataLayer.Data;
using LearningProject1.DataLayer.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

var appName = builder.Configuration["AppSettings:AppName"];

var app = builder.Build();

app.Logger.LogInformation("App name: {AppName}", appName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// Global error handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is BadRequestException)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(exception.Message);
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
        }
    });
});

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
