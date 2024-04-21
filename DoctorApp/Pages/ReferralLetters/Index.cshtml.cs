using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DoctorApp.Pages.ReferralLetters
{
	public class IndexModel : PageModel
	{
		private readonly DataContext _context;
		public IndexModel(DataContext context)
		{
			_context = context;
		}

		public List<ReferralLetter> ReferralLetters { get; set; }

		public async Task OnGetAsync()
		{
			if (_context.Specialties != null)
			{
				ReferralLetters = await _context.ReferralLetters.ToListAsync();
			}
		}
	}
}
