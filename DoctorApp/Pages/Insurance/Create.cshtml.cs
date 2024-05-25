using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DoctorApp.Models;
using DoctorApp.Data;
using DoctorApp.Utilities;

namespace DoctorApp.Pages.Insurance
{
	public class CreateModel : PageModel
	{
		private readonly DataContext _context;
		public CreateModel(DataContext context)
		{
			_context = context;
		}
		public IActionResult OnGet()
		{
			return Page();
		}
		[BindProperty]
		public InsuranceCompany InsuranceCompanies { get; set; }

		public async Task<IActionResult> OnPost()
		{
			if (!ModelState.IsValid || _context.InsuranceCompanies == null || InsuranceCompanies == null)
			{
				return Page();
			}

			var existingInsuranceCompanyName = await _context.InsuranceCompanies
				.Where(i => EF.Functions.Like(i.CompanyName, InsuranceCompanies.CompanyName))
				.FirstOrDefaultAsync();

			if (existingInsuranceCompanyName != null)
			{
				ModelState.AddModelError("InsuranceCompanies.CompanyName", "Insurance Company Name already exists");
				return Page();
			}

			// Set additional properties
			InsuranceCompanies.CreatedBy = "joyce";
			InsuranceCompanies.CompanyName = StringExtensions
				.CapitalizeLetters(InsuranceCompanies.CompanyName);

			_context.InsuranceCompanies.Add(InsuranceCompanies);
			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}

	}
}