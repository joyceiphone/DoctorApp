using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Doctors
{
    public class CreateModel : PageModel
    {
		private readonly DataContext _context;

		public CreateModel(DataContext context)
		{
			_context = context;
		}
		[BindProperty]
		public Doctor Doctors { get; set; }
		public List<SelectListItem> Options { get; set; }

		public async Task OnGetAsync()
		{

			Options = await _context.Specialties.Select(a =>
								  new SelectListItem
								  {
									  Value = a.Id.ToString(),
									  Text = a.SpecialityName
								  }).ToListAsync();
		}

		public async Task<IActionResult> OnPost()
		{
			if (!ModelState.IsValid || _context.Doctors == null || Doctors == null)
			{
				return Page();
			}

			Doctors.ModifiedBy ="joyce";
			Doctors.CreatedBy = "joyce";
			_context.Doctors.Add(Doctors);
			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}
    }
}
