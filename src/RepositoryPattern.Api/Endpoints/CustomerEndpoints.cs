using Microsoft.AspNetCore.Mvc;
using RepositoryPattern.Api.Helper;
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
                        ? ApiResponseResults.NotFound()
                        : ApiResponseResults.Ok(customer);
                }
                catch (Exception ex)
                {
                    return ApiResponseResults.Fail(StatusCodes.Status500InternalServerError, ex.Message);
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
                    return ApiResponseResults.Created<object>(null, $"Criado customer com id: {id}");
                }
                catch (Exception ex)
                {
                    await uow.RollbackAsync(cancelationToken);
                    return ApiResponseResults.Fail(StatusCodes.Status500InternalServerError, ex.Message);
                }
            });

        app.MapGet("/customers", async (IRepository<Customer> repository) =>
        {
            try
            {
                var customers = await repository.ListAsync();
                return ApiResponseResults.Ok(customers);
            }
            catch (Exception ex)
            {
                return ApiResponseResults.Fail(StatusCodes.Status500InternalServerError, ex.Message);
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
                //return updated ? Results.NoContent() : Results.BadRequest();
                return updated
                    ? ApiResponseResults.Ok<object>(null, $"Customer {id} atualizado com sucesso")
                    : ApiResponseResults.Fail(StatusCodes.Status400BadRequest, $"Customer {id} não atualizado");
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync(cancelationToken);
                return ApiResponseResults.Fail(StatusCodes.Status500InternalServerError, ex.Message);
            }
        });

        app.MapDelete("/customers/{id:int}", async (int id, IRepository<Customer> repository, IUnitWork uow,
                CancellationToken cancelationToken) =>
        {
            try
            {
                await uow.BeginAsync(cancelationToken);
                var deleted = await repository.DeleteAsync(id, cancelationToken);
                await uow.CommitAsync(cancelationToken);
                return deleted 
                    ? ApiResponseResults.Ok<object>(null, $"Customer excluido da base.") 
                    : ApiResponseResults.Fail(StatusCodes.Status400BadRequest, $"Erro ao excluir customer: {id}.");
            }
            catch (Exception ex)
            {
                return ApiResponseResults.Fail(StatusCodes.Status500InternalServerError, ex.Message);
            }
 
        });
        
        return app;
        
    }
    
    

}