﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace DoctorApp.Models
{
	public class ReferralLetter
	{
		public ReferralLetter()
		{
			CreatedBy = "DefaultUser";
		}
		[Key]
		public int ID { get; set; }
		public int DoctorID { get; set; }
		public string PtAccNumber { get; set; }
		public string FileName { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? ModifiedDateTime { get; set; }
		public string? ModifiedBy { get; set; }
		public bool IsActive { get; set; } = true;
		public DateTime? DeletedDateTime { get; set; }
		public string? DeletedBy { get; set; }
		public Doctor Doctor { get; set; } = null!;
	}
}
