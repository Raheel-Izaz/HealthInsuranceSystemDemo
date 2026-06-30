namespace HealthInsuranceSystem.Domain.Entities
{
    public class InstallmentPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public int Count { get; set; } 
        public bool IsActive { get; set; } = true;
    }
}
