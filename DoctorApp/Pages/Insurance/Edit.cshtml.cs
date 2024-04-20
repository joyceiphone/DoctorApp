using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Insurance
{
	public class EditModel : PageModel
	{
		private readonly DataContext _context;

		public EditModel(DataContext context)
		{
			_context = context;
		}
		[BindProperty]
		public InsuranceCompany InsuranceCompanies { get; set; }
		public async Task<IActionResult> OnGet(int? itemid)
		{
			if (itemid == null || _context.InsuranceCompanies == null)
			{
				return NotFound();
			}
			var insurance = await _context.InsuranceCompanies.FirstOrDefaultAsync(p => p.Id == itemid);

			if (insurance == null)
			{
				return NotFound();
			}
			InsuranceCompanies = insurance;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			InsuranceCompanies.ModifiedDateTime = DateTime.Now;
			InsuranceCompanies.ModifiedBy = "test";

			_context.InsuranceCompanies.Update(InsuranceCompanies);
			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}
	}
}