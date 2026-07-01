using HealthInsuranceSystem.Domain.Entities;
using HealthInsuranceSystem.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Controllers
{
    public class InstallmentPlanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstallmentPlanController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var plans = await _context.InstallmentPlans.ToListAsync();
            return View(plans);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstallmentPlan plan)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid configuration data submitted.";
                return RedirectToAction(nameof(Index));
            }

            if (plan.Count < 1 || plan.Count > 24)
            {
                TempData["Error"] = "Installment count configuration must be between 1 and 24 payments.";
                return RedirectToAction(nameof(Index));
            }

            _context.InstallmentPlans.Add(plan);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Billing plan '{plan.Name}' with {plan.Count} iterations successfully initialized.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _context.InstallmentPlans.FindAsync(id);
            if (plan == null)
            {
                TempData["Error"] = "The requested billing plan could not be located.";
                return RedirectToAction(nameof(Index));
            }

            bool isPlanInUse = await _context.Policies.AnyAsync(p => p.InstallmentPlanId == id);
            if (isPlanInUse)
            {
                TempData["Error"] = $"Cannot delete '{plan.Name}'. It is currently linked to active insurance policies.";
                return RedirectToAction(nameof(Index));
            }

            _context.InstallmentPlans.Remove(plan);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Billing plan '{plan.Name}' was successfully removed from global settings.";
            return RedirectToAction(nameof(Index));
        }
    }
}
