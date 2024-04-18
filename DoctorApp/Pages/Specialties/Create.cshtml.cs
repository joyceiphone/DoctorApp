using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DoctorApp.Pages.Specialties
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
			// Set additional properties
			Specialties.DeletedBy = "joyce";
			Specialties.CreatedBy = "joyce";

			// Add the specialty to the context and save changes
			_context.Specialties.Add(Specialties);
			await _context.SaveChangesAsync();

			return RedirectToPage(nameof(Index));
		}

	}
}
