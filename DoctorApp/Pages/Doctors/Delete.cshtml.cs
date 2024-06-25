using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Doctors
{
	public class DeleteModel : PageModel
	{
		private readonly DataContext _context;

		public DeleteModel(DataContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Doctor Doctors { get; set; }

		public List<string> InsuranceCompanies { get; set; } // Declare InsuranceCompanies property

		public JoinedResult JoinedResults { get; set; }

		[BindProperty]
		public List<Address> Addresses { get; set; }

		public async Task<IActionResult> OnGetAsync(int? itemid)
		{
			if (itemid == null)
			{
				return NotFound();
			}

			InsuranceCompanies = await (from t1 in _context.Doctors
										join t2 in _context.InsuranceCompanies_Doctors on t1.Id equals t2.DoctorId
										join t3 in _context.InsuranceCompanies on t2.InsuranceCompanyId equals t3.Id
										where t1.Id == itemid & t1.IsActive & t2.IsActive & t3.IsActive
										select t3.CompanyName).ToListAsync();

			var doctor = await (from t1 in _context.Doctors
								join t2 in _context.Specialties on t1.SpecialityID equals t2.Id
								where t1.Id == itemid && t1.IsActive
								select new JoinedResult
								{
									Id = t1.Id,
									DrFName = t1.DrFName,
									DrLName = t1.DrLName,
									SpecialityName = t2.SpecialityName,
									EmailPersonal = t1.EmailPersonal,
									EmailWork = t1.EmailWork,
									PersonalCell = t1.PersonalCell
								}).FirstOrDefaultAsync();

			if (doctor == null)
			{
				return NotFound();
			}

			Addresses = await _context.Addresses
				.Where(p => p.DoctorId == itemid && p.IsActive).ToListAsync();
			JoinedResults = doctor;
			return Page();
		}
		public async Task<IActionResult> OnPostAsync(int? itemid)
		{
			if (itemid == null)
			{
				return NotFound();
			}
			var doctor = await _context.Doctors.FindAsync(itemid);
			if (doctor == null)
			{
				return NotFound();
			}
			doctor.IsActive = false;
			doctor.DeletedDateTime = DateTime.UtcNow;
			doctor.DeletedBy = "DefaultUser";
			Doctors = doctor;
			_context.Doctors.Update(Doctors);

			var insuranceCompanyDoctors = await _context.InsuranceCompanies_Doctors
				.Where(p => p.DoctorId == itemid && p.IsActive)
				.ToListAsync();

			if(insuranceCompanyDoctors != null)
			{
				foreach(var companyDoctor in insuranceCompanyDoctors)
				{
					companyDoctor.IsActive = false;
					companyDoctor.DeletedDateTime = DateTime.UtcNow;
					companyDoctor.DeletedBy = "DefaultUser";
				}
				_context.InsuranceCompanies_Doctors.UpdateRange(insuranceCompanyDoctors);
			}

			Addresses = await _context.Addresses
				.Where(p => p.DoctorId == itemid && p.IsActive).ToListAsync();

			if (Addresses != null)
			{
				foreach (var address in Addresses)
				{
					address.DeletedDateTime = DateTime.UtcNow;
					address.DeletedBy = "DefaultUser";
					address.IsActive = false;
					_context.Addresses.Update(address);
				}
			}

			await _context.SaveChangesAsync();
			return RedirectToPage(nameof(Index));
		}
		public class JoinedResult
		{
			public int Id { get; set; }
			public string DrFName { get; set; }
			public string DrLName { get; set; }
			public string SpecialityName { get; set; }
			public string EmailPersonal { get; set; }
			public string EmailWork { get; set; }
			public string PersonalCell { get; set; }
		}
	}
}
