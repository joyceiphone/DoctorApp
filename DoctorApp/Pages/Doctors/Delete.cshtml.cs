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

		public async Task<IActionResult> OnGetAsync(int? itemid)
		{
			if (itemid == null)
			{
				return NotFound();
			}

			InsuranceCompanies = await (from t1 in _context.Doctors
										join t2 in _context.InsuranceCompanies_Doctors on t1.Id equals t2.DoctorId
										join t3 in _context.InsuranceCompanies on t2.InsuranceCompanyId equals t3.Id
										where t1.Id == itemid
										select t3.CompanyName).ToListAsync();

			var doctor = await (from t1 in _context.Doctors
								join t2 in _context.Specialties on t1.SpecialityID equals t2.Id
								where t1.Id == itemid
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

			if(doctor == null)
			{
				return NotFound();
			}

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
			doctor.IsActive = true;
			Doctors = doctor;
			_context.Doctors.Remove(Doctors);
			await _context.SaveChangesAsync();

			var insuranceCompanyDoctors = await _context.InsuranceCompanies_Doctors
				.Where(p => p.DoctorId == itemid)
				.ToListAsync();

			_context.InsuranceCompanies_Doctors.RemoveRange(insuranceCompanyDoctors);
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
