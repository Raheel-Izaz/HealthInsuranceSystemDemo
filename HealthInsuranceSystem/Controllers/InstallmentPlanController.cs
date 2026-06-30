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

        // 1. GET: /InstallmentPlan/Index
        // Displays the plan setup screen (List of current configurations + Creation Form)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var plans = await _context.InstallmentPlans.ToListAsync();
            return View(plans);
        }

        // 2. POST: /InstallmentPlan/Create
        // Saves a brand new configuration structure straight to the SQL table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstallmentPlan plan)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid configuration data submitted.";
                return RedirectToAction(nameof(Index));
            }

            // Business Validation Check: Ensure count is realistic
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
    }
}
