using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Identity;
using static DoctorApp.Pages.Doctors.EditModel;

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

		public List<JoinedResultModel> InsuranceCompaniesDoctors { get; set; }

		public async Task OnGetAsync()
		{
			if (_context.Specialties != null)
			{
				InsuranceCompanies = await _context.InsuranceCompanies.
					Include(i=>i.Doctors).Where(i=>i.IsActive).ToListAsync();

				InsuranceCompaniesDoctors = await (from t1 in _context.Doctors
												   join t2 in _context.InsuranceCompanies_Doctors on t1.Id equals t2.DoctorId
												   join t3 in _context.InsuranceCompanies on t2.InsuranceCompanyId equals t3.Id
												   group new { t2 } by new { t3.Id, t3.CompanyName } into g
												   select new JoinedResultModel
												   {
													   Id = g.Key.Id,
													   CompanyName = g.Key.CompanyName,
													   IsActiveCount = g.Count(x => x.t2.IsActive),
												   }).ToListAsync();
			}
		}
	}
	public class JoinedResultModel
	{
		public int Id { get; set; }
		public string CompanyName{ get; set; }

		public int IsActiveCount { get; set; }
	}
}