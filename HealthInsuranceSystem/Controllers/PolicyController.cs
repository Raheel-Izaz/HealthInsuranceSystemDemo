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

        // GET: /Policy/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Eager load the Company profile and all related Installments using .Include()
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

            // Fetch only currently approved/active payment configurations
            ViewBag.PlanList = await _context.InstallmentPlans.Where(p => p.IsActive).ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Policy policy)
        {
            // Find the chosen configuration parameters 
            var selectedPlan = await _context.InstallmentPlans.FindAsync(policy.InstallmentPlanId);
            if (selectedPlan == null) return BadRequest("Invalid installment plan chosen.");

            // Map the actual configuration count straight to the transaction data record
            policy.InstallmentCount = selectedPlan.Count;

            await _policyService.CreatePolicyAndScheduleAsync(policy);

            return RedirectToAction("Index", "Company");
        }
    }
}
