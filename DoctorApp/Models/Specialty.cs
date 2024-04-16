﻿using System.ComponentModel.DataAnnotations;

namespace DoctorApp.Models
{
	public class Specialty
	{
		[Key]
		public int Id { get; set; }
		public string SpecialityName { get; set; }
		public DateTime CreatedDateTime { get; set; }
		public string CreatedBy { get; set; }
		public DateTime ModifiedDateTime { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public DateTime DeletedDateTime { get; set; }
		public string DeletedBy { get; set; }

		public ICollection<Doctor> Doctors { get; } = new List<Doctor>();
	}
}
