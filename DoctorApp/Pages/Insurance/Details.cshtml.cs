using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Insurance
{
	public class DetailsModel : PageModel
	{
		private readonly DataContext _context;

		public DetailsModel(DataContext context)
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
	}
}