using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DoctorApp.Data;
using DoctorApp.Models;

namespace DoctorApp.Pages.Doctors
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
		public Specialty Specialties { get; set; }

		public async Task<IActionResult> OnPost()
		{
			if (!ModelState.IsValid || _context.Specialties == null || Specialties == null)
			{
				return Page();
			}
			_context.Specialties.Add(Specialties);
			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}
    }
}
