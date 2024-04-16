using Microsoft.EntityFrameworkCore;
using DoctorApp.Models;
using System.Reflection.Metadata;

namespace DoctorApp.Data
{
	public class DataContext: DbContext
	{
		public DataContext(DbContextOptions<DataContext> options): base(options) { }

		public DbSet<Doctor> Doctors;
		public DbSet<InsuranceCompany> InsuranceCompanies;
		public DbSet<InsuranceCompany_Doctor> InsuranceCompanies_Doctors;
		public DbSet<Address> Addresses;
		public DbSet<Specialty> Specialties;
		public DbSet<ReferralLetter> ReferralLetters;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//many to many
			modelBuilder.Entity<Doctor>()
				.HasMany(e => e.InsuranceCompanies)
				.WithMany(e => e.Doctors)
				.UsingEntity<InsuranceCompany_Doctor>();

			//one to many
			modelBuilder.Entity<Doctor>()
				.HasMany(e => e.Addresses)
				.WithOne(e => e.Doctor)
				.HasForeignKey(e => e.DoctorId)
				.IsRequired();

			//one to many
			modelBuilder.Entity<Specialty>()
				.HasMany(e => e.Doctors)
				.WithOne(e => e.Specialty)
				.HasForeignKey(e => e.SpecialityID)
				.IsRequired();

			//one to many
			modelBuilder.Entity<Doctor>()
                .HasMany(e => e.ReferralLetters)
				.WithOne(e => e.Doctor)
				.HasForeignKey(e => e.DoctorID)
				.IsRequired();
        }
	}
}
