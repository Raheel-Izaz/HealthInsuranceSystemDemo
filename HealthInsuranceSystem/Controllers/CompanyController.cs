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
        // 1. GET: /Company/Index
        // Displays the dashboard containing the Excel Upload Form and the list of active Companies
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Fetch all companies from the infrastructure database layer
            var companies = await _context.Companies
                .Include(p=>p.Policies)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(companies);
        }
        // 2. POST: /Company/UploadExcel
        // Processes the uploaded spreadsheet file stream using ClosedXML
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects your endpoint from CSRF attacks
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            // Validation: Check if a valid file was provided
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid Excel file before clicking upload.";
                return RedirectToAction(nameof(Index));
            }

            // Validation: Restrict file extension types to excel formats
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                TempData["Error"] = "Unsupported file format. Please upload a standard Excel spreadsheet (.xlsx).";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var companiesList = new List<Company>();

                // Open the file stream using a scoped 'using' block to prevent server memory leaks
                using (var stream = file.OpenReadStream())
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        // Access the first worksheet inside the workbook
                        var worksheet = workbook.Worksheet(1);

                        // Grab all populated rows, skipping row 1 (the table header labels)
                        var rows = worksheet.RowsUsed().Skip(1);

                        foreach (var row in rows)
                        {
                            // Assuming Column 1 contains the Company Name string value
                            var companyName = row.Cell(1).GetString()?.Trim();

                            // Skip empty rows to protect database data integrity
                            if (string.IsNullOrWhiteSpace(companyName)) continue;

                            companiesList.Add(new Company
                            {
                                Name = companyName,
                                CreatedBy = "Super Admin (Excel Batch)"
                            });
                        }
                    }
                }

                // Database Optimization: Check if we parsed actual records before hitting SQL Server
                if (companiesList.Any())
                {
                    // AddRange batches all elements together as a single mass collection insert
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
                // Fail-safe error tracking logging
                TempData["Error"] = $"Critical processing failure during file parsing: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));

        }
    }
}