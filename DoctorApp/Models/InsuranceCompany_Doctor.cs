using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorApp.Models
{
	public class InsuranceCompany_Doctor
	{
		public InsuranceCompany_Doctor()
		{
			CreatedBy = "DefaultUser"; // Set default value for CreatedBy
			ModifiedBy = "DefaultUser";
		}

		[Key]
		public int Id { get; set; }
		public int DoctorId { get; set; }
		public int InsuranceCompanyId { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public DateTime ? DeletedDateTime { get; set; }
		public string ? DeletedBy { get; set; }
	}
}
