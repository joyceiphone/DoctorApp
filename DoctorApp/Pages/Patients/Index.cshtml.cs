using DoctorApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Hl7.Fhir.Model;

namespace DoctorApp.Pages.Patients
{
	public class IndexModel : PageModel
	{
		private readonly FhirService _fhirService;
		public IndexModel(FhirService fhirService)
		{
			_fhirService = fhirService;
		}
		[BindProperty]
		public List<Patient> PatientList { get; set; }
		[BindProperty]
		public String LastName { get; set; }
		[BindProperty]
		public String FirstName { get; set; }

		[BindProperty]
		public String BirthDate { get; set; }

		[BindProperty]
		public String PatientId { get; set; }
		public async Task<IActionResult> OnGetAsync()
		{
			PatientList = await _fhirService.GetPatients();

			if (PatientList == null)
			{
				return NotFound();
			}

			return Page();
		}
	}
}

