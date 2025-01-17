﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorApp.Models
{
	public class Doctor
	{
		public Doctor()
		{
			CreatedBy = "DefaultUser";
		}
		[Key]
		public int Id { get; set; }

		[DisplayName("First Name")]
		public string DrFName { get; set; }

		[DisplayName("Last Name")]
		public string DrLName { get; set; }

		[DisplayName("Middle Name")]
		public string? DrMName { get; set; }
		public int? SpecialityID { get; set; }

		[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your Email is not valid.")]
		public string? EmailPersonal { get; set; }

		[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your Email is not valid.")]
		public string? EmailWork { get; set; }

		[RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Please enter a valid phone number in the format XXX-XXX-XXXX.")]
		public string? PersonalCell { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime? CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? ModifiedDateTime { get; set; }
		public string? ModifiedBy { get; set; }
		public bool IsActive { get; set; } = true;
		public DateTime? DeletedDateTime { get; set; }
		public string? DeletedBy { get; set; }
		public ICollection<InsuranceCompany> InsuranceCompanies { get; } = new List<InsuranceCompany>();
		public ICollection<ReferralLetter> ReferralLetters { get; } = new List<ReferralLetter>();
		public ICollection<Address> Addresses { get; } = new List<Address>();
		public Specialty? Specialty { get; set; } = null!;
	}
}
