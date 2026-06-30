using HealthInsuranceSystem.Domain.Entities;

namespace HealthInsuranceSystem.Application
{
    public interface IPolicyService
    {
        Task CreatePolicyAndScheduleAsync(Policy policy);
    }
}
