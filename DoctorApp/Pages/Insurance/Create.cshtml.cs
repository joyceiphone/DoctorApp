using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DoctorApp.Models;
using DoctorApp.Data;

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

			// Set additional properties
			InsuranceCompanies.DeletedBy = "joyce";
			InsuranceCompanies.CreatedBy = "joyce";
			InsuranceCompanies.IsActive = false;

			_context.InsuranceCompanies.Add(InsuranceCompanies);
			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}

	}
}