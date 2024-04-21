using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Doctors
{
	public class EditModel : PageModel
	{
		private readonly DataContext _context;
		public EditModel(DataContext context)
		{
			_context = context;
		}

		[BindProperty]
		public int DoctorId { get; set; }
		[BindProperty]
		public int AddressId { get; set; }
		[BindProperty]
		public Doctor Doctors { get; set; }
		[BindProperty]
		public List<SelectListItem> Options { get; set; }

		[BindProperty]
		public List<SelectListItem> Companies { get; set; }
		public List<String> InsuranceCompanies { get; set; }

		[BindProperty]
		public Address Addresses { get; set; }

		public List<InsuranceCompany_Doctor> InsuranceCompanies_Doctors { get; set; }
		public JoinedResult JoinedResults { get; set; }

		public async Task<IActionResult> OnGetAsync(int? itemid)
		{
			if (itemid == null)
			{
				return NotFound();
			}

			var doctor = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == itemid);
			if (doctor == null)
			{
				return NotFound();

			}

			Doctors = doctor;

			DoctorId = doctor.Id;

			Options = await _context.Specialties.Select(a =>
					  new SelectListItem
					  {
						  Value = a.Id.ToString(),
						  Text = a.SpecialityName
					  }).ToListAsync();

			InsuranceCompanies = await (from t1 in _context.Doctors
										join t2 in _context.InsuranceCompanies_Doctors on t1.Id equals t2.DoctorId
										join t3 in _context.InsuranceCompanies on t2.InsuranceCompanyId equals t3.Id
										where t1.Id == itemid
										select new JoinedInsuranceCompany
										{
											Id = t2.Id,
											CompanyName = t3.CompanyName
										}
										).Select(ic => ic.CompanyName).ToListAsync();

			Companies = await _context.InsuranceCompanies.Select(a =>
					  new SelectListItem
					  {
						  Value = a.Id.ToString(),
						  Text = a.CompanyName
					  }).ToListAsync();

			Addresses = await _context.Addresses.FirstOrDefaultAsync(p => p.DoctorId == itemid);
			if (Addresses != null)
			{
				AddressId = Addresses.Id;
			}

			return Page();
		}

		public async Task<IActionResult> OnPost(int? itemid)
		{
			if (!ModelState.IsValid || _context.Doctors == null || Doctors == null)
			{
				return Page();
			}

			Doctors.Id = DoctorId;
			Doctors.ModifiedBy = "joyce";
			Doctors.CreatedBy = "joyce";
			Doctors.ModifiedDateTime = DateTime.Now;
			Doctors.IsActive = false;
			_context.Doctors.Update(Doctors);

			// Get the list of checked insurance companies
			var checkedCompanies = Request.Form["CheckedCompanies"].ToList();

			// Get the previously stored list of insurance companies associated with the doctor
			var existingCompanies = await _context.InsuranceCompanies_Doctors
				.Where(ic => ic.DoctorId == DoctorId)
				.Select(ic => ic.InsuranceCompanyId.ToString())
				.ToListAsync();

			// Delete rows for unchecked companies that were previously checked
			var uncheckedCompanies = existingCompanies.Except(checkedCompanies);
			foreach (var companyId in uncheckedCompanies)
			{
				var rowToDelete = await _context.InsuranceCompanies_Doctors
					.FirstOrDefaultAsync(ic => ic.DoctorId == DoctorId && ic.InsuranceCompanyId.ToString() == companyId);
				if (rowToDelete != null)
				{
					rowToDelete.IsActive = true;
					_context.InsuranceCompanies_Doctors.Remove(rowToDelete);
				}
			}

			// Add rows for checked companies that were previously unchecked
			var newlyCheckedCompanies = checkedCompanies.Except(existingCompanies);
			foreach (var companyId in newlyCheckedCompanies)
			{
				_context.InsuranceCompanies_Doctors.Add(new InsuranceCompany_Doctor
				{

					DoctorId = DoctorId,
					InsuranceCompanyId = Convert.ToInt32(companyId),
					IsActive = false
				});
			}

			if (AddressId != 0)
			{
				Addresses.Id = AddressId;
				Addresses.DoctorId = DoctorId;
				Addresses.ModifiedDateTime = DateTime.Now;
				_context.Addresses.Update(Addresses);
			}
			else
			{
				Addresses.DoctorId = DoctorId;
				Addresses.IsActive = false;
				_context.Addresses.Add(Addresses);
			}

			await _context.SaveChangesAsync();

			return RedirectToPage(nameof(Index));
		}


		public class JoinedResult
		{
			public int Id { get; set; }
			public string DrFName { get; set; }

			public int SpecialtyId { get; set; }
			public string DrLName { get; set; }
			public string SpecialityName { get; set; }
			public string EmailPersonal { get; set; }
			public string EmailWork { get; set; }
			public string PersonalCell { get; set; }
		}

		public class JoinedInsuranceCompany
		{
			public int Id { get; set; }
			public string CompanyName { get; set; }
		}
	}
}
