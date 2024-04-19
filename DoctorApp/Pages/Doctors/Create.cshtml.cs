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

		public List<SelectListItem> Companies { get; set; }
		[BindProperty]
		public List<int> InsuranceCompaniesIds { get; set; }
		public List<InsuranceCompany_Doctor> InsuranceCompany_Doctors { get; set; }

		public async Task OnGetAsync()
		{

			Options = await _context.Specialties.Select(a =>
								  new SelectListItem
								  {
									  Value = a.Id.ToString(),
									  Text = a.SpecialityName
								  }).ToListAsync();
			Companies = await _context.InsuranceCompanies.Select(a =>
								  new SelectListItem
								  {
									  Value = a.Id.ToString(),
									  Text = a.CompanyName
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
			Doctors.IsActive = false;
			_context.Doctors.Add(Doctors);
			await _context.SaveChangesAsync();

			int newDoctorId = Doctors.Id;

			foreach(var id in InsuranceCompaniesIds)
			{
				var insuranceCompanyDoctor = new InsuranceCompany_Doctor
				{
					InsuranceCompanyId = id,
					DoctorId = newDoctorId,

				};

				_context.InsuranceCompanies_Doctors.Add(insuranceCompanyDoctor);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage(nameof(Index));
		}
    }
}
