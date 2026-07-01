using ClosedXML.Excel;
using HealthInsuranceSystem.Domain.Entities;
using HealthInsuranceSystem.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Controllers
{
    public class CompanyController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }
         [HttpGet]
        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies
                .Include(p=>p.Policies)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(companies);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid Excel file before clicking upload.";
                return RedirectToAction(nameof(Index));
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                TempData["Error"] = "Unsupported file format. Please upload a standard Excel spreadsheet (.xlsx).";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var companiesList = new List<Company>();

                using (var stream = file.OpenReadStream())
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        var rows = worksheet.RowsUsed().Skip(1);

                        foreach (var row in rows)
                        {
                            var companyName = row.Cell(1).GetString()?.Trim();

                            if (string.IsNullOrWhiteSpace(companyName)) continue;

                            companiesList.Add(new Company
                            {
                                Name = companyName,
                                CreatedBy = "Super Admin (Excel Batch)"
                            });
                        }
                    }
                }

                if (companiesList.Any())
                {
                    await _context.Companies.AddRangeAsync(companiesList);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"{companiesList.Count} corporate accounts successfully ingested into the system.";
                }
                else
                {
                    TempData["Error"] = "The uploaded spreadsheet did not contain any valid data rows.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Critical processing failure during file parsing: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));

        }
    }
}