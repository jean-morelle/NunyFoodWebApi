using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Customers;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Customers.Queries;

/// <summary>Retourne null si le client n'existe pas ou n'est pas accessible à l'utilisateur courant.</summary>
public record GetCustomerByIdQuery(Guid Id) : IQuery<CustomerDto?>;

public class GetCustomerByIdQueryHandler(IRepository<Customer> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        if (!currentUser.CanAccessCustomer(request.Id)) return null;

        var c = await repository.GetByIdAsync(request.Id, ct);
        return c is null ? null : mapper.Map<CustomerDto>(c);
    }
}
