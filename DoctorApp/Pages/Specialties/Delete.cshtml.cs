using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoctorApp.Models;
using DoctorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Specialties
{
    public class DeleteModel : PageModel
    {
		private readonly DataContext _context;

		public DeleteModel(DataContext context)
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
		public async Task<IActionResult> OnPostAsync(int? itemid)
		{
			if (itemid == null)
			{
				return NotFound();
			}
			var specialty = await _context.Specialties.FindAsync(itemid);
			if (specialty == null)
			{
				return NotFound();
			}
			Specialties = specialty;
			_context.Remove(specialty);
			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}
	}
}
