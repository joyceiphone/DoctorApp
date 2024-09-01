using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using DoctorApp.Models;

namespace DoctorApp.Services
{
    public class FhirService
    {
        private readonly FhirClient _fhirClient;

        public FhirService(string fhirEndpoint)
        {
            _fhirClient = new FhirClient(fhirEndpoint);
        }

        public async Task<List<Patient>> GetPatients()
        {
            List<Patient> patients = new List<Patient>();

            try
            {
                var searchParams = new SearchParams();
                var response = await _fhirClient.SearchAsync<Patient>(searchParams);

                patients = response.Entry
                    .Where(entry => entry?.Resource is Patient)
                    .Select(entry => entry.Resource as Patient)
                    .ToList();

            }
            catch (FhirOperationException ex)
            {
                Console.WriteLine($"FHIR Operation Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return patients;
        }

		private static PatientViewModel MapToPatientViewModel(Patient patient)
		{
			return new PatientViewModel
			{
				Id = patient.Id,
				FamilyName = patient.Name.FirstOrDefault()?.Family,
				GivenName = patient.Name.FirstOrDefault()?.Given.FirstOrDefault(),
				BirthDate = patient.BirthDate,
				Gender = patient.Gender?.ToString(),
			};
		}
	}
}
