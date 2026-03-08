using RepositoryPattern.Api.Endpoints;
using RepositoryPattern.Application.Contracts;
using RepositoryPattern.Domain.Entities;
using RepositoryPattern.Infraestructure.Data.Abstractions;
using RepositoryPattern.Infraestructure.Data.Core;
using RepositoryPattern.Infraestructure.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionFactory, SqlConnectionFactory>();

builder.Services.AddScoped<DbSessions>();
builder.Services.AddScoped<IUnitWork, UnitOfWork>();

builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();

var app = builder.Build();

app.MapGet("/", () => "Welcome to RepositoryPatternApi!");

app.MapCustomerEndpoints();

app.Run();
