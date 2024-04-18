using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace DoctorApp.Models
{
	public class ReferralLetter
	{
		public ReferralLetter()
		{
			CreatedBy = "DefaultUser"; // Set default value for CreatedBy
			ModifiedBy = "DefaultUser";
		}
		[Key]
		public int ID { get; set; }
		public int DoctorID { get; set; }
		public string PtAccNumber { get; set; }
		public string FileName { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime ModifiedDateTime { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public DateTime ? DeletedDateTime { get; set; }
		public string ? DeletedBy { get; set; }
		public Doctor Doctor { get; set; } = null!;
    }
}
