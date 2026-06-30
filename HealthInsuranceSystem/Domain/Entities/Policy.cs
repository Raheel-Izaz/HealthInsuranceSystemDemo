namespace HealthInsuranceSystem.Domain.Entities
{
    public class Policy
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal TotalPremium { get; set; }
        public int InstallmentCount { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        public ICollection<Installment> Installments { get; set; } = new List<Installment>();

        public int InstallmentPlanId { get; set; }
        public InstallmentPlan? InstallmentPlan { get; set; }
    }
}
