using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Specialties
{
	public class DetailsModel : PageModel
	{
		private readonly DataContext _context;

		public DetailsModel(DataContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Specialty Specialties { get; set; }
		public async Task<IActionResult> OnGetAsync(int? itemid)
		{
			if (itemid == null)
			{
				return NotFound();
			}
			var specialty = await _context.Specialties.FirstOrDefaultAsync(p => p.Id == itemid);

			if (specialty == null)
			{
				return NotFound();
			}
			Specialties = specialty;
			return Page();
		}
	}
}
