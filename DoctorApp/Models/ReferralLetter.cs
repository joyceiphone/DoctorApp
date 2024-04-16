using System.ComponentModel.DataAnnotations;

namespace DoctorApp.Models
{
	public class ReferralLetter
	{
		[Key]
		public int ID { get; set; }
		public int DoctorID { get; set; }
		public string PtAccNumber { get; set; }
		public string FileName { get; set; }
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime ModifiedDateTime { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public DateTime DeletedDateTime { get; set; }
		public string DeletedBy { get; set; }
	}
}
