using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Insurance
{
    public class IndexModel : PageModel
    {
		private readonly DataContext _context;

		public IndexModel(DataContext context)
		{
			_context = context;
		}

		public List<InsuranceCompany> InsuranceCompanies { get; set; }

		public async Task OnGetAsync()
		{
			if (_context.Specialties != null)
			{
				InsuranceCompanies = await _context.InsuranceCompanies.ToListAsync();
			}
		}
	}
}
