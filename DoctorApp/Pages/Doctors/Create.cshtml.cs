using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Net.Http;
using System.Security.Policy;
using System.Xml.Linq;
using System;
using Azure.Core;
using System.Net.Http.Headers;
using System.Text.Json;
using Azure;

namespace DoctorApp.Pages.Doctors
{
	public class CreateModel : PageModel
	{
		private readonly DataContext _context;
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public CreateModel(DataContext context, HttpClient httpClient, IConfiguration configuration)
		{
			_context = context;
			_httpClient = httpClient;
			_configuration = configuration;
		}
		[BindProperty]
		public Doctor Doctors { get; set; }
		public List<SelectListItem> Options { get; set; }

		public List<SelectListItem> Companies { get; set; }

		public List<InsuranceCompany_Doctor> InsuranceCompany_Doctors { get; set; }

		[BindProperty]
		public List<Address> Addresses { get; set; }

		public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
		{
			await OnGetAsync();
			await base.OnPageHandlerExecutionAsync(context, next);
		}
		public async Task OnGetAsync()
		{
			if (Addresses == null)
			{
				Addresses = new List<Address>();
			}

			if (!Addresses.Any())
			{
				Addresses.Add(new Address());
			}
			Options =
			[
				new SelectListItem { Value = "", Text = "Select Specialty" },
				.. await _context.Specialties.Where(s=>s.IsActive).Select(a =>
									  new SelectListItem
									  {
										  Value = a.Id.ToString(),
										  Text = a.SpecialityName
									  }).ToListAsync(),
			];

			Companies = await _context.InsuranceCompanies.Where(i => i.IsActive).Select(a =>
								  new SelectListItem
								  {
									  Value = a.Id.ToString(),
									  Text = a.CompanyName
								  }).ToListAsync();
		}

		public IActionResult OnGetAddRow()
		{
			Addresses.Add(new Address());
			return Partial("_AddressPartial", Addresses.Last());
		}

		public async Task<IActionResult> OnGetValidateAddress(string zipCode)
		{
			if (string.IsNullOrEmpty(zipCode))
			{
				return BadRequest("ZipCode is required.");
			}
			Console.WriteLine("initiate sending request");

			var accessToken = _configuration["USPS:AccessToken"];
			var apiUrl = $"https://api.usps.com/addresses/v3/city-state?zipcode={zipCode}";

			Console.WriteLine("initiate sending request");

			try
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				var response = await _httpClient.GetAsync(apiUrl);

				Console.WriteLine(response.Content);
				Console.WriteLine("initiate sending request");

				if (response.IsSuccessStatusCode)
				{
					var jsonContent = await response.Content.ReadAsStringAsync();
					var result = JsonSerializer.Deserialize<CityStateResponse>(jsonContent);

					return new JsonResult(new { City = result.City, State = result.State });
				}
				else
				{
					Console.WriteLine(response.Content);
					Console.WriteLine("initiate sending request");
					return StatusCode((int)response.StatusCode, response.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}

		}

		public async Task<IActionResult> OnPost(List<Address> addresses)
		{
			if (Doctors.SpecialityID == null)
			{
				ModelState.AddModelError("Doctors.SpecialityID", "Specialty is required");
			}

			if (!ModelState.IsValid || _context.Doctors == null || Doctors == null)
			{
				return Page();
			}

			_context.Doctors.Add(Doctors);

			await _context.SaveChangesAsync();

			int newDoctorId = Doctors.Id;

			var checkedCompanies = Request.Form["CheckedCompanies"].ToList();

			foreach (var id in checkedCompanies)
			{
				var insuranceCompanyDoctor = new InsuranceCompany_Doctor
				{
					InsuranceCompanyId = Convert.ToInt32(id),
					DoctorId = newDoctorId,

				};

				_context.InsuranceCompanies_Doctors.Add(insuranceCompanyDoctor);
				await _context.SaveChangesAsync();
			}

			foreach (var address in addresses)
			{
				address.DoctorId = newDoctorId;
				_context.Addresses.Add(address);
			}

			await _context.SaveChangesAsync();

			return RedirectToPage(nameof(Index));
		}
	}

	public class CityStateResponse
	{
		public string City { get; set; }
		public string State { get; set; }
	}
}