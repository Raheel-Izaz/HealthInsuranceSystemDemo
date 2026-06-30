namespace HealthInsuranceSystem.Domain.Entities
{
    public class Installment
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; } = false;
       
    }
}
