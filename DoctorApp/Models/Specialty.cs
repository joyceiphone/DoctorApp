using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace DoctorApp.Models
{
	public class Specialty
	{
		public Specialty()
		{
			CreatedBy = "DefaultUser"; // Set default value for CreatedBy
			ModifiedBy = "DefaultUser";
		}
		[Key]
		public int Id { get; set; }
		public string SpecialityName { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime ModifiedDateTime { get; set; }

		public string ModifiedBy { get; set; }
		public bool? IsActive { get; set; }
		public DateTime? DeletedDateTime { get; set; }
		public string? DeletedBy { get; set; }
		public ICollection<Doctor> Doctors { get; } = new List<Doctor>();
	}
}
