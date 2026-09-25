using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Beneficiaries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Beneficiaries.Queries;

public record GetBeneficiariesByCustomerQuery(Guid CustomerId) : IQuery<IEnumerable<BeneficiaryDto>>;

public class GetBeneficiariesByCustomerQueryHandler(IRepository<Beneficiary> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<GetBeneficiariesByCustomerQuery, IEnumerable<BeneficiaryDto>>
{
    public async Task<IEnumerable<BeneficiaryDto>> Handle(GetBeneficiariesByCustomerQuery request, CancellationToken ct)
    {
        // Un client ne voit que ses propres bénéficiaires, quel que soit le paramètre envoyé.
        var customerId = currentUser.IsCustomer ? currentUser.UserId : request.CustomerId;

        var list = await repository.FindAsync(b => b.CustomerId == customerId, ct);
        return mapper.Map<IEnumerable<BeneficiaryDto>>(list);
    }
}
