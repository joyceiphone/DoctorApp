using DoctorApp.Models;

namespace DoctorApp.Pages.Doctors
{
	internal class InsuranceCompanies_Doctors : InsuranceCompany_Doctor
	{
		public int DoctorId { get; set; }
		public object InsuranceCompanyId { get; set; }
	}
}