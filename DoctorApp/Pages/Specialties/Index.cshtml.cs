using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Specialties
{
	public class IndexModel : PageModel
	{
		private readonly DataContext _context;

		public IndexModel(DataContext context)
		{
			_context = context;
		}

		public List<Specialty> Specialties { get; set; }

		public async Task OnGetAsync()
		{
			if (_context.Specialties != null)
			{
				Specialties = await _context.Specialties
					.Include(s=>s.Doctors).ToListAsync();
			}
		}

	}
}
