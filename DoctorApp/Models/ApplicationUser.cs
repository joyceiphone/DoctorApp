using Microsoft.AspNetCore.Identity;

namespace DoctorApp.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FName { get; set; } = "";
		public string LName { get; set; } = "";
		public string Address { get; set; } = "";
		public DateTime CreatedDateTime { get; set; }

    }
}
