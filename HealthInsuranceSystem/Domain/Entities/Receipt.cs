namespace HealthInsuranceSystem.Domain.Entities
{
    public class Receipt
    {
        public int Id { get; set; }
        public int InstallmentId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = "Bank Transfer";
    }
}
