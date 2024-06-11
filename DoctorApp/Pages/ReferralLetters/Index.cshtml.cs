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

		public List<JoinedResultModel> ReferralLetters { get; set; }

		public async Task OnGetAsync()
		{
			if (_context.Specialties != null)
			{
				ReferralLetters = await (from t1 in _context.ReferralLetters
										 join t2 in _context.Doctors on t1.DoctorID equals t2.Id
										 where t1.IsActive & t2.IsActive
										 select new JoinedResultModel
										 {
											 Id = t1.ID,
											 DrFName = t2.DrFName,
											 DrLName = t2.DrLName,
											 FileName = t1.FileName,
											 PtAccNumber = t1.PtAccNumber,
											 CreatedDateTime = t1.CreatedDateTime,
										 }).ToListAsync();
			}
		}
	}
	public class JoinedResultModel
	{
		public int Id { get; set; }
		public string DrFName { get; set; }
		public string DrLName { get; set; }
		public string FileName { get; set; }

		public string PtAccNumber { get; set; }

		public DateTime CreatedDateTime { get; set; }
	}
}
