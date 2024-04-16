using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DoctorApp.Models
{
	public class Doctor
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
    }
}
