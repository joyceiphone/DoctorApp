using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorApp.Models
{
	public class Address
	{
		public Address()
		{
			CreatedBy = "DefaultUser";
		}
		[Key]
		public int Id { get; set; }
		public int DoctorId { get; set; }
		public string Street1 { get; set; }
		public string? Street2 { get; set; }
		public string ZipCode { get; set; }
		public string City { get; set; }

		[RegularExpression("^[A-Z]{2}$", ErrorMessage = "State is required.")]
		public string State { get; set; }

		[RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Please enter a valid phone number in the format XXX-XXX-XXXX.")]
		public string TelAddress { get; set; }

		[RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Please enter a valid phone number in the format XXX-XXX-XXXX.")]
		public string? FaxAddress { get; set; }
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? ModifiedDateTime { get; set; }
		public string? ModifiedBy { get; set; }
		public bool IsActive { get; set; } = true;
		public DateTime? DeletedDateTime { get; set; }
		public string? DeletedBy { get; set; }
		public Doctor? Doctor { get; set; } = null!;
	}
}