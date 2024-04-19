using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoctorApp.Models;
using DoctorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.Specialties
{
    public class EditModel : PageModel
    {
        private readonly DataContext _context;

        public EditModel(DataContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Specialty Specialties{ get; set; }
        public async Task<IActionResult> OnGet(int? itemid)
        {
            if (itemid == null || _context.Specialties == null)
            {
                return NotFound();
            }
            var product = await _context.Specialties.FirstOrDefaultAsync(p => p.Id == itemid);

            if (product == null)
            {
                return NotFound();
            }
            Specialties = product;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Specialties.ModifiedDateTime = DateTime.Now;
            Specialties.ModifiedBy = "test";
            _context.Specialties.Update(Specialties);
            await _context.SaveChangesAsync();
            return RedirectToPage(nameof(Index));
        }
    }
}
