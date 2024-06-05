using Microsoft.EntityFrameworkCore;
using DoctorApp.Models;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DoctorApp.Data
{
	public class DataContext : IdentityDbContext<ApplicationUser>
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options) { }

		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<InsuranceCompany> InsuranceCompanies { get; set; }
		public DbSet<InsuranceCompany_Doctor> InsuranceCompanies_Doctors { get; set; }
		public DbSet<Address> Addresses { get; set; }
		public DbSet<Specialty> Specialties { get; set; }
		public DbSet<ReferralLetter> ReferralLetters { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//Identity roles
			var admin = new IdentityRole("admin");
			admin.NormalizedName = "admin";

			var client = new IdentityRole("client");
			client.NormalizedName = "client";

			modelBuilder.Entity<IdentityRole>().HasData(admin, client);

			//many to many
			modelBuilder.Entity<Doctor>()
				.HasMany(e => e.InsuranceCompanies)
				.WithMany(e => e.Doctors)
				.UsingEntity<InsuranceCompany_Doctor>();

			modelBuilder.Entity<Doctor>()
				.Property(s => s.CreatedDateTime)
				.HasDefaultValueSql("GETDATE()");

			modelBuilder.Entity<Address>()
					.Property(s => s.CreatedDateTime)
					.HasDefaultValueSql("GETDATE()");

			modelBuilder.Entity<InsuranceCompany>()
					.Property(s => s.CreatedDateTime)
					.HasDefaultValueSql("GETDATE()");

			modelBuilder.Entity<InsuranceCompany_Doctor>()
					.Property(s => s.CreatedDateTime)
					.HasDefaultValueSql("GETDATE()");

			modelBuilder.Entity<ReferralLetter>()
					.Property(s => s.CreatedDateTime)
					.HasDefaultValueSql("GETDATE()");

			modelBuilder.Entity<Specialty>()
					.Property(s => s.CreatedDateTime)
					.HasDefaultValueSql("GETDATE()");

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