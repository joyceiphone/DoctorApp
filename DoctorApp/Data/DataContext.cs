using Microsoft.EntityFrameworkCore;
using DoctorApp.Models;

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
			modelBuilder.Entity<Doctor>()
				.HasMany(e => e.InsuranceCompanies)
				.WithMany(e => e.Doctors)
				.UsingEntity<InsuranceCompany_Doctor>();
		}
	}
}
