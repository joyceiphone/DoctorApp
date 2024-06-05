using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorApp.Models
{
	public class InsuranceCompany
	{
		public InsuranceCompany()
		{
			CreatedBy = "DefaultUser";
		}
		[Key]
		public int Id { get; set; }
		[DisplayName("Company Name")]
		public string CompanyName { get; set; }
		public string? Department { get; set; }

		[RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Please enter a valid phone number in the format XXX-XXX-XXXX.")]
		public string? Telephone { get; set; }
		[DisplayName("Contact Person")]

		public string? ContactPerson { get; set; }
		[DisplayName("Contact Email")]
		[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your Email is not valid.")]
		public string? ContactEmail { get; set; }
		public string? Note { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? ModifiedDateTime { get; set; }
		public string? ModifiedBy { get; set; }
		public bool IsActive { get; set; } = true;
		public DateTime? DeletedDateTime { get; set; }
		public string? DeletedBy { get; set; }
		public ICollection<Doctor> Doctors { get; } = [];
	}
}
