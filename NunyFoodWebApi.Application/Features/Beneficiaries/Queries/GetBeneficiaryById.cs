using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Beneficiaries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Beneficiaries.Queries;

/// <summary>Retourne null si le bénéficiaire n'existe pas ou appartient à un autre client.</summary>
public record GetBeneficiaryByIdQuery(Guid Id) : IQuery<BeneficiaryDto?>;

public class GetBeneficiaryByIdQueryHandler(IRepository<Beneficiary> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<GetBeneficiaryByIdQuery, BeneficiaryDto?>
{
    public async Task<BeneficiaryDto?> Handle(GetBeneficiaryByIdQuery request, CancellationToken ct)
    {
        var b = await repository.GetByIdAsync(request.Id, ct);
        return b is null || !currentUser.CanAccessCustomer(b.CustomerId) ? null : mapper.Map<BeneficiaryDto>(b);
    }
}
