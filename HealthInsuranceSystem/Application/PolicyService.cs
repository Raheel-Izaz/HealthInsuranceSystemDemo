using HealthInsuranceSystem.Domain.Entities;
using HealthInsuranceSystem.Infrastructure;

namespace HealthInsuranceSystem.Application
{
    public class PolicyService : IPolicyService
    {
        private readonly ApplicationDbContext _context;
        public PolicyService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreatePolicyAndScheduleAsync(Policy policy)
        {
            decimal individualAmount = policy.TotalPremium / policy.InstallmentCount;

            for (int i = 0; i < policy.InstallmentCount; i++)
            {
                policy.Installments.Add(new Installment
                {
                    Amount = individualAmount,
                    DueDate = DateTime.UtcNow.AddMonths(i),
                    IsPaid = false
                });
            }

            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();
        }
    }
}
