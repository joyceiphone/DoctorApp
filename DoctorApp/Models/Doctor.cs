using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorApp.Models
{
	public class Doctor
	{
		public Doctor()
		{
			CreatedBy = "DefaultUser"; // Set default value for CreatedBy
			ModifiedBy = "DefaultUser";
		}
		[Key]
		public int Id { get; set; }
		public string DrFName { get; set; }
		public string DrLName { get; set; }
		public string? DrMName { get; set; }
		public int SpecialityID { get; set; }
		public string EmailPersonal { get; set; }
		public string EmailWork { get; set; }
		public string PersonalCell { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime ModifiedDateTime { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public DateTime? DeletedDateTime { get; set; }
		public string? DeletedBy { get; set; }
		public ICollection<InsuranceCompany> InsuranceCompanies { get; } = new List<InsuranceCompany>();
		public ICollection<ReferralLetter> ReferralLetters { get; } = new List<ReferralLetter>();
		public ICollection<Address> Addresses { get; } = new List<Address>();
		public Specialty? Specialty { get; set; } = null!;
	}
}
