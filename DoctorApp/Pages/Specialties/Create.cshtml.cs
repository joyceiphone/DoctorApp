using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DoctorApp.Utilities;

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

			var existingSpecialty = await _context.Specialties
				.Where(s => EF.Functions.Like(s.SpecialityName, Specialties.SpecialityName))
				.FirstOrDefaultAsync();

			if(existingSpecialty != null)
			{
				ModelState.AddModelError("Specialties.SpecialityName", "Specialty Name already exists");
				return Page();
			}

			// Set additional properties
			Specialties.CreatedBy = "joyce";
			Specialties.SpecialityName = StringExtensions.
				CapitalizeFirstLetter(Specialties.SpecialityName);

			if(Specialties.SpecialityName.ToUpper() == "ENT")
			{
				Specialties.SpecialityName = StringExtensions.CapitalizeLetters
					(Specialties.SpecialityName);
			}

			// Add the specialty to the context and save changes
			_context.Specialties.Add(Specialties);
			await _context.SaveChangesAsync();

			return RedirectToPage(nameof(Index));
		}

	}
}
