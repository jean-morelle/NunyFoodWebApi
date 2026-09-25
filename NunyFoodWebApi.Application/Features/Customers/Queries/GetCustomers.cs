using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Customers;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Customers.Queries;

public record GetCustomersQuery : IQuery<IEnumerable<CustomerDto>>;

public class GetCustomersQueryHandler(IRepository<Customer> repository, IMapper mapper)
    : IRequestHandler<GetCustomersQuery, IEnumerable<CustomerDto>>
{
    public async Task<IEnumerable<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        var list = await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<CustomerDto>>(list);
    }
}
