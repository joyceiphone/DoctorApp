using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Doctors
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;

        public IndexModel(DataContext context)
        {
            _context = context;
        }

        public List<SelectListItem> Options { get; set; }
		public List<Doctor> Doctors { get; set; }
        public List<Specialty> Specialities { get; set; }

		public async Task OnGetAsync()
		{
			Options = await _context.Specialties.Select(a =>
								  new SelectListItem
								  {
									  Value = a.Id.ToString(),
									  Text = a.SpecialityName
								  }).ToListAsync();
			Specialities = await _context.Specialties.ToListAsync();
		}
    }
}
