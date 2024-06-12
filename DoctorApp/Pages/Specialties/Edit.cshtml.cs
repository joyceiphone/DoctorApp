using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoctorApp.Models;
using DoctorApp.Data;
using Microsoft.EntityFrameworkCore;
using DoctorApp.Utilities;
using System.Text.RegularExpressions;

namespace DoctorApp.Pages.Specialties
{
	public class EditModel : PageModel
	{
		private readonly DataContext _context;

		public EditModel(DataContext context)
		{
			_context = context;
		}
		[BindProperty]
		public Specialty Specialties { get; set; }
		public async Task<IActionResult> OnGet(int? itemid)
		{
			if (itemid == null || _context.Specialties == null)
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

		public async Task<IActionResult> OnPostAsync(int itemId)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var existingSpecialty = await _context.Specialties
				.Where(s => EF.Functions.Like(s.SpecialityName, Specialties.SpecialityName) & s.IsActive)
				.FirstOrDefaultAsync();

			if (existingSpecialty != null & existingSpecialty.Id != itemId)
			{
				ModelState.AddModelError("Specialties.SpecialityName", "Specialty Name already exists");
				return Page();
			}


			Specialties.SpecialityName = Specialties.SpecialityName.Trim();

			if (Specialties.SpecialityName.ToUpper() == "ENT")
			{
				Specialties.SpecialityName = StringExtensions.CapitalizeLetters
					(Specialties.SpecialityName);
			}

			Specialties.ModifiedDateTime = DateTime.Now;
			Specialties.ModifiedBy = "test";
			_context.Specialties.Update(Specialties);
			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}
	}
}
