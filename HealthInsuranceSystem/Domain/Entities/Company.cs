namespace HealthInsuranceSystem.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = "Admin";
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}
