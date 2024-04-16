using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DoctorApp.Pages.Doctors
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;

        public IndexModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string selectedSpecialty { get; set; }
        public List<Doctor> Doctors { get; set; }
        public async Task OnGetAsync()
        {
            var selectedItem = Request.Form["specialty"];

            if(selectedItem == "Show All")
            {
                var Doctors = await _context.Doctors.ToListAsync();
            }
            else
            {
                var Doctors = await _context.Doctors.Where(p => p.SpecialityID == selectedItem).ToListAsync();
            }

        }
    }
}
