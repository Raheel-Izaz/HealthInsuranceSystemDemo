using HealthInsuranceSystem.Application;
using HealthInsuranceSystem.Domain.Entities;
using HealthInsuranceSystem.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Controllers
{
    public class PolicyController : Controller
    {
        private readonly IPolicyService _policyService;
        private readonly ApplicationDbContext _context;
        public PolicyController(ApplicationDbContext context, IPolicyService policyService)
        {
            _policyService = policyService;
            _context = context;
        }
        public IActionResult Index()
        {
            var policies = _context.Policies
                .Include(p => p.Company)
                .Include(p => p.Installments)
                .OrderByDescending(p => p.Id)
                .ToList();
            return View(policies);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var policy = await _context.Policies
                .Include(p => p.Company)
                .Include(p => p.Installments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (policy == null)
            {
                TempData["Error"] = "The requested policy contract could not be located.";
                return RedirectToAction("Index", "Company");
            }

            return View(policy);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.CompanyList = await _context.Companies.ToListAsync();

            ViewBag.PlanList = await _context.InstallmentPlans.Where(p => p.IsActive).ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Policy policy)
        {
            var selectedPlan = await _context.InstallmentPlans.FindAsync(policy.InstallmentPlanId);
            if (selectedPlan == null) return BadRequest("Invalid installment plan chosen.");

            policy.InstallmentCount = selectedPlan.Count;

            await _policyService.CreatePolicyAndScheduleAsync(policy);

            return RedirectToAction("Index", "Company");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var policy = await _context.Policies
                .Include(p => p.Installments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (policy == null)
            {
                TempData["Error"] = "The requested policy contract could not be found.";
                return RedirectToAction("Index", "Company");
            }

            if (policy.Installments != null && policy.Installments.Any())
            {
                _context.Installments.RemoveRange(policy.Installments);
            }

            _context.Policies.Remove(policy);
            await _context.SaveChangesAsync(); 

            TempData["Success"] = $"Policy '{policy.PolicyNumber}' and all related automated installments have been purged safely.";
            return RedirectToAction("Index", "Company");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CollectPayment(int installmentId, int policyId)
        {
            var installment = await _context.Installments.FindAsync(installmentId);
            if (installment == null)
            {
                TempData["Error"] = "Installment record not found.";
                return RedirectToAction("Details", new { id = policyId });
            }

            if (installment.IsPaid)
            {
                TempData["Error"] = "This installment has already been settled.";
                return RedirectToAction("Details", new { id = policyId });
            }

            installment.IsPaid = true;

            var receipt = new Receipt
            {
                InstallmentId = installmentId,
                AmountPaid = installment.Amount,
                PaymentDate = DateTime.UtcNow
            };

            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Payment successfully received! Receipt reference generated.";
            return RedirectToAction("Details", new { id = policyId });
        }
    }
}
