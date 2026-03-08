using Microsoft.AspNetCore.Mvc;
using RepositoryPattern.Application.Contracts;
using RepositoryPattern.Domain.Entities;

namespace RepositoryPattern.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/customers/{id:int}",
            async (int id, IRepository<Customer> repository, CancellationToken cancellationToken) =>
            {
                try
                {
                    var customer = await repository.GetByIdAsync(id, cancellationToken);
                    return customer is null
                        ? Results.NotFound()
                        : Results.Ok(customer);
                }
                catch (Exception ex)
                {
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            });

        app.MapPost("/customers",
            async (Customer input, IRepository<Customer> repository, IUnitWork uow,
                CancellationToken cancelationToken) =>
            {
                try
                {
                    await uow.BeginAsync(cancelationToken);
                    var id = await repository.AddAsync(input, cancelationToken);
                    await uow.CommitAsync(cancelationToken);
                    return Results.Created($"/customers/{id}", new { id });
                }
                catch (Exception ex)
                {
                    await uow.RollbackAsync(cancelationToken);
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            });

        app.MapGet("/customers", async (IRepository<Customer> repository) =>
        {
            try
            {
                var customers = await repository.ListAsync();
                return Results.Ok(customers);
            }
            catch (Exception ex)
            {
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        });

        app.MapPut("/customers/{id:int}",
            async (int id,
                [FromBody] Customer input,
                IRepository<Customer> repository,
                IUnitWork uow,
                CancellationToken cancelationToken) =>
        {
            try
            {
                input.Id = id;
                await uow.BeginAsync(cancelationToken);
                var updated = await repository.UpdateAsync(input, cancelationToken);
                await uow.CommitAsync(cancelationToken);
                return updated ? Results.NoContent() : Results.BadRequest();
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync(cancelationToken);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        });

        app.MapDelete("/customers/{id:int}", async (int id, IRepository<Customer> repository, IUnitWork uow,
                CancellationToken cancelationToken) =>
        {
            await uow.BeginAsync(cancelationToken);
            var deleted = await repository.DeleteAsync(id, cancelationToken);
            await uow.CommitAsync(cancelationToken);
            return deleted ? Results.NoContent() : Results.BadRequest();
        });
        
        return app;
        
    }
    
    

}