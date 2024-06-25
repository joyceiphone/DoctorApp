using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
		public Doctor Doctors { get; set; }
		[BindProperty]
		public List<SelectListItem> Options { get; set; }

		[BindProperty]
		public List<SelectListItem> Companies { get; set; }
		public List<String> InsuranceCompanies { get; set; }

		[BindProperty]
		public List<Address> Addresses { get; set; }

		public List<int> AddressIds { get; set; }

		public List<InsuranceCompany_Doctor> InsuranceCompanies_Doctors { get; set; }
		public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
		{
			int? itemid = Convert.ToInt32(context.HttpContext.Request.Query["itemid"].FirstOrDefault());
			await OnGetAsync(itemid);
			await base.OnPageHandlerExecutionAsync(context, next);
		}

		public async Task<IActionResult> OnGetAsync(int? itemid)
		{
			if (itemid == null)
			{
				return NotFound();
			}

			Doctors = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == itemid);

			if (Doctors == null)
			{
				return NotFound();

			}

			Options = [
				new SelectListItem { Value = "", Text = "Select Specialty" },
				.. await _context.Specialties.Where(s=>s.IsActive).Select(a =>
									  new SelectListItem
									  {
										  Value = a.Id.ToString(),
										  Text = a.SpecialityName
									  }).ToListAsync(),
			];

			InsuranceCompanies = await (from t1 in _context.Doctors
										join t2 in _context.InsuranceCompanies_Doctors on t1.Id equals t2.DoctorId
										join t3 in _context.InsuranceCompanies on t2.InsuranceCompanyId equals t3.Id
										where t1.Id == itemid & t1.IsActive & t2.IsActive & t3.IsActive
										select new JoinedInsuranceCompany
										{
											Id = t2.Id,
											CompanyName = t3.CompanyName
										}
										).Select(ic => ic.CompanyName).ToListAsync();

			Companies = await _context.InsuranceCompanies
				.Where(i => i.IsActive).Select(a =>
					  new SelectListItem
					  {
						  Value = a.Id.ToString(),
						  Text = a.CompanyName
					  }).ToListAsync();

			Addresses = await _context.Addresses
				.Where(p => p.DoctorId == itemid && p.IsActive).ToListAsync();

			if (Addresses == null)
			{
				Addresses = new List<Address>();
			}

			if (!Addresses.Any())
			{
				Addresses.Add(new Address());
			}

			return Page();
		}

		public async Task<IActionResult> OnGetAddRow()
		{
			int? itemid = Request.RouteValues["itemid"] as int?;
			Addresses = await _context.Addresses
				.Where(p => p.DoctorId == itemid && p.IsActive).ToListAsync();
			if (Addresses != null)
			{
				foreach (var Address in Addresses)
				{
					AddressIds.Add(Address.Id);
				}
			}

			// Check if Addresses is null before adding a new Address
			if (Addresses == null)
			{
				Addresses = new List<Address>();
			}

			Addresses.Add(new Address());
			return Partial("_AddressPartial", Addresses.Last());
		}

		public async Task<IActionResult> OnPost(int? itemid, Doctor doctors, List<Address> addresses)
		{
			if (doctors.SpecialityID == null)
			{
				ModelState.AddModelError("Doctors.SpecialityID", "Specialty is required");
			}

			if (!ModelState.IsValid || _context.Doctors == null || Doctors == null)
			{
				return Page();
			}

			doctors.ModifiedDateTime = DateTime.UtcNow;
			doctors.ModifiedBy = "DefaultUser";
			_context.Doctors.Update(doctors);

			// Get the list of checked insurance companies
			var checkedCompanies = Request.Form["CheckedCompanies"].ToList();

			// Get the previously stored list of insurance companies associated with the doctor
			var existingCompanies = await _context.InsuranceCompanies_Doctors
				.Where(ic => ic.DoctorId == Doctors.Id && ic.IsActive)
				.Select(ic => ic.InsuranceCompanyId.ToString())
				.ToListAsync();

			// Delete rows for unchecked companies that were previously checked
			var uncheckedCompanies = existingCompanies.Except(checkedCompanies);
			foreach (var companyId in uncheckedCompanies)
			{
				var rowToDelete = await _context
					.InsuranceCompanies_Doctors
					.FirstOrDefaultAsync(ic => ic.DoctorId == Doctors.Id
					&& ic.InsuranceCompanyId.ToString() == companyId
					&& ic.IsActive
					);
				if (rowToDelete != null)
				{
					rowToDelete.IsActive = false;
					rowToDelete.DeletedDateTime = DateTime.UtcNow;
					rowToDelete.DeletedBy = "DefaultUser";
					_context.InsuranceCompanies_Doctors.Update(rowToDelete);
				}
			}

			// Add rows for checked companies that were previously unchecked
			var newlyCheckedCompanies = checkedCompanies.Except(existingCompanies);
			foreach (var companyId in newlyCheckedCompanies)
			{
				_context.InsuranceCompanies_Doctors.Add(new InsuranceCompany_Doctor
				{
					DoctorId = Doctors.Id,
					InsuranceCompanyId = Convert.ToInt32(companyId),
				});
			}

			var addressIds = await _context.Addresses
				.Where(p => p.DoctorId == itemid && p.IsActive)
				.Select(p => p.Id).ToListAsync();

			var updatedIds = new List<int>();

			foreach (var address in addresses)
			{
				if (address.Id != 0)
				{
					updatedIds.Add(address.Id);
					var existingAddress = await _context.Addresses.FindAsync(address.Id);
					if (existingAddress.Street1 != address.Street1 ||
					existingAddress.Street2 != address.Street2 ||
					existingAddress.City != address.City ||
					existingAddress.State != address.State ||
					existingAddress.ZipCode != address.ZipCode ||
					existingAddress.TelAddress != address.TelAddress ||
					existingAddress.FaxAddress != address.FaxAddress
					)
					{
						address.DoctorId = Doctors.Id;
						address.Id = existingAddress.Id;
						address.ModifiedDateTime = DateTime.UtcNow;
						address.ModifiedBy = "DefaultUser";
						_context.Addresses.Update(address);
					}
				}
				else
				{
					address.DoctorId = Doctors.Id;
					_context.Addresses.Add(address);
				}
			}

			if (addressIds != null)
			{
				var addressIdsToDelete = addressIds.Except(updatedIds);
				if (addressIdsToDelete != null)
				{
					foreach (var id in addressIdsToDelete)
					{
						var addressToDelete = await _context.Addresses.FindAsync(Convert.ToInt32(id));
						if (addressToDelete != null)
						{
							addressToDelete.IsActive = false;
							addressToDelete.DeletedDateTime = DateTime.UtcNow;
							addressToDelete.DeletedBy = "DefaultUser";
							_context.Update(addressToDelete);
						}
					}
				}
			}

			await _context.SaveChangesAsync();


			return RedirectToPage(nameof(Index));
		}

		public class JoinedInsuranceCompany
		{
			public int Id { get; set; }
			public string CompanyName { get; set; }
		}
	}
}
