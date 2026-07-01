using HealthInsuranceSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Installment> Installments { get; set; }
        public DbSet<InstallmentPlan> InstallmentPlans { get; set; }
        public DbSet<Receipt> Receipts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InstallmentPlan>().HasData(
                new InstallmentPlan { Id = 1, Name = "Lump Sum / Annual", Count = 1, IsActive = true },
                new InstallmentPlan { Id = 2, Name = "Semi-Annual", Count = 2, IsActive = true },
                new InstallmentPlan { Id = 3, Name = "Quarterly", Count = 4, IsActive = true },
                new InstallmentPlan { Id = 4, Name = "Monthly", Count = 12, IsActive = true }
            );
        }
    }
}