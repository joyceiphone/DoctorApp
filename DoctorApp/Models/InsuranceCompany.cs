using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DoctorApp.Models
{
	public class InsuranceCompany
	{
		[Key]
		public int Id { get; set; }
		[DisplayName("Company Name")]
		public string CompanyName { get; set; }
		public string Department { get; set; }
		public string Telephone { get; set; }
		[DisplayName("Contact Person")]
		public string ContactPerson { get; set; }
		[DisplayName("Contact Email")]
		public string ContactEmail { get; set; }
		public string Note { get; set; }
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime ModifiedDateTime { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public DateTime DeletedDateTime { get; set; }
		public string DeletedBy { get; set; }
		public List<Doctor> Doctors { get; } = [];
	}
}
