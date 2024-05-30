using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;

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

		public List<InsuranceCompany_Doctor> InsuranceCompany_Doctors { get; set; }

		[BindProperty]
		public List<Address> Addresses { get; set; }

		public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
		{
			await OnGetAsync();
			await base.OnPageHandlerExecutionAsync(context, next);
		}
		public async Task OnGetAsync()
		{
			if (Addresses == null)
			{
				Addresses = new List<Address>();
			}

			if (!Addresses.Any())
			{
				Addresses.Add(new Address());
			}
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

		public IActionResult OnGetAddRow()
		{
			Addresses.Add(new Address());
			return Partial("_AddressPartial", Addresses.Last());
		}

		public async Task<IActionResult> OnPost(List<Address> addresses)
		{
			if (!ModelState.IsValid || _context.Doctors == null || Doctors == null)
			{
				return Page();
			}

			Doctors.ModifiedBy = "joyce";
			Doctors.CreatedBy = "joyce";
			_context.Doctors.Add(Doctors);

			await _context.SaveChangesAsync();

			int newDoctorId = Doctors.Id;

			var checkedCompanies = Request.Form["CheckedCompanies"].ToList();

			foreach (var id in checkedCompanies)
			{
				var insuranceCompanyDoctor = new InsuranceCompany_Doctor
				{
					InsuranceCompanyId = Convert.ToInt32(id),
					DoctorId = newDoctorId,

				};

				insuranceCompanyDoctor.ModifiedBy = "joyce";
				insuranceCompanyDoctor.CreatedBy = "joyce";

				_context.InsuranceCompanies_Doctors.Add(insuranceCompanyDoctor);
				await _context.SaveChangesAsync();
			}

			foreach (var address in addresses)
			{
				address.DoctorId = newDoctorId;
				_context.Addresses.Add(address);
			}

			await _context.SaveChangesAsync();

			return RedirectToPage(nameof(Index));
		}
	}
}