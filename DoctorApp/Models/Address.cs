using System.ComponentModel.DataAnnotations;

namespace DoctorApp.Models
{
	public class Address
	{
		[Key]
		public int Id { get; set; }
		public int DoctorId { get; set; }
		public string Street1 { get; set; }
        public string Street2 { get; set; }
		public string ZipCode { get; set; }
		public string City { get; set; }
		public string State { get; set; }
        public string TelAddress { get; set; }
		public string FaxAddress { get; set; }
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime ModifiedDateTime { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public DateTime DeletedDateTime { get; set; }
		public string DeletedBy { get; set; }
	}
}
