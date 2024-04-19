using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Insurance
{
    public class DeleteModel : PageModel
    {
		private readonly DataContext _context;

		public DeleteModel(DataContext context)
		{
			_context = context;
		}

		[BindProperty]
		public InsuranceCompany InsuranceCompanies { get; set; }
		public async Task<IActionResult> OnGetAsync(int? itemid)
		{
			if (itemid == null)
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
		public async Task<IActionResult> OnPostAsync(int? itemid)
		{
			if (itemid == null)
			{
				return NotFound();
			}
			var insurance = await _context.InsuranceCompanies.FindAsync(itemid);
			if (insurance == null)
			{
				return NotFound();
			}
			insurance.IsActive = true;
			InsuranceCompanies = insurance;
			_context.Remove(insurance);
			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}
	}
}
