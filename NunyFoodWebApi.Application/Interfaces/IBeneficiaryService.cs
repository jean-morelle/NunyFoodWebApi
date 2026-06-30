using NunyFoodWebApi.Application.DTOs.Beneficiaries;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IBeneficiaryService
{
    Task<BeneficiaryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<BeneficiaryDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<BeneficiaryDto> CreateAsync(CreateBeneficiaryDto dto, CancellationToken ct = default);
    Task<BeneficiaryDto?> UpdateAsync(Guid id, UpdateBeneficiaryDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
