using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;


namespace DoctorApp.Pages.Doctors
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;

        public IndexModel(DataContext context)
        {
            _context = context;
        }

        public List<SelectListItem> Options { get; set; }
		public List<Doctor> Doctors { get; set; }
        public List<Specialty> Specialities { get; set; }
		public List<JoinedResultModel> JoinedResult { get; set; }


		[BindProperty]
		public int selectedId { get; set; }

        [BindProperty]
        public List<string> CheckedItems { get; set; }

		public async Task OnGetAsync()
		{

			Options = await _context.Specialties.Select(a =>
								  new SelectListItem
								  {
									  Value = a.Id.ToString(),
									  Text = a.SpecialityName
								  }).ToListAsync();
			Options.Add(new SelectListItem { Value = "0", Text = "Show All" });

			JoinedResult = await (from t1 in _context.Doctors
								  join t2 in _context.Specialties
								  on t1.SpecialityID equals t2.Id
								  select new JoinedResultModel
								  {
									  Id= t1.Id,
									  DrFName = t1.DrFName,
									  DrLName = t1.DrLName,
									  SpecialityName = t2.SpecialityName,
								  }).ToListAsync();
		}

		public async Task<IActionResult> OnPost(string command)
		{
			Options = await _context.Specialties.Select(a =>
								  new SelectListItem
								  {
									  Value = a.Id.ToString(),
									  Text = a.SpecialityName
								  }).ToListAsync();
			Options.Add(new SelectListItem { Value = "0", Text = "Show All" });

			if (command == "FilterAction")
			{
				// Logic for Action 1

				if (selectedId == 0)
				{
					JoinedResult = await (from t1 in _context.Doctors
										  join t2 in _context.Specialties
										  on t1.SpecialityID equals t2.Id
										  select new JoinedResultModel
										  {
											  Id = t1.Id,
											  DrFName = t1.DrFName,
											  DrLName = t1.DrLName,
											  SpecialityName = t2.SpecialityName
										  }).ToListAsync();
				}
				else
				{
					JoinedResult = await (from t1 in _context.Doctors
										  join t2 in _context.Specialties
										  on t1.SpecialityID equals t2.Id
										  where t1.SpecialityID == selectedId
										  select new JoinedResultModel
										  {
											  Id = t1.Id,
											  DrFName = t1.DrFName,
											  DrLName = t1.DrLName,
											  SpecialityName = t2.SpecialityName
										  }).ToListAsync();
				}
			}
			if(command == "PrintCheckedItems")
			{
				JoinedResult = await (from t1 in _context.Doctors
									  join t2 in _context.Specialties
									  on t1.SpecialityID equals t2.Id
									  select new JoinedResultModel
									  {
										  Id = t1.Id,
										  DrFName = t1.DrFName,
										  DrLName = t1.DrLName,
										  SpecialityName = t2.SpecialityName
									  }).ToListAsync();

				if (CheckedItems != null && CheckedItems.Any())
				{
                    // Create a new PDF document.
                    var document = new PdfDocument();
					document.Info.Title = "Created with PDFsharp";
					document.Info.Subject = "Just a simple Hello-World program.";

					// Create an empty page in this document.
					var page = document.AddPage();
					//page.Size = PageSize.Letter;

					// Get an XGraphics object for drawing on this page.
					var gfx = XGraphics.FromPdfPage(page);

					var width = page.Width;
					var height = page.Height;

                    var font = new XFont("Times New Roman", 12, XFontStyleEx.Bold);
                    int y = 20;

                    var checkedIds = Request.Form["CheckedItems"].ToList();

                    // Retrieve the details of the checked items from the database
                    var checkedItems = await (from t1 in _context.Doctors
                                              join t2 in _context.Specialties on t1.SpecialityID equals t2.Id
                                              where checkedIds.Contains(t1.Id.ToString())
                                              select new JoinedResultModel
                                              {
                                                  DrFName = t1.DrFName,
                                                  DrLName = t1.DrLName,
                                                  SpecialityName = t2.SpecialityName
                                              }).ToListAsync();

                    foreach (var item in checkedItems)
					{
						gfx.DrawString($"Specialty: {item.SpecialityName}", font, XBrushes.Black, new XPoint(50, y));
						y += 20;
                        gfx.DrawString($"{item.DrFName} {item.DrLName} MD", font, XBrushes.Black, new XPoint(50, y));
                        y += 20;
                        y += 40;
					};

					document.Save("testPdfSharp.pdf");

					using (var stream = new MemoryStream())
					{
						document.Save(stream, false);
						stream.Position = 0;

						// Return the PDF file as a FileResult.
						return File(stream.ToArray(), "application/pdf", "CheckedDoctors.pdf");
					}
				}
			}

			return Page();
		}

		public class JoinedResultModel
		{
			public int Id { get; set; }
			public string DrFName { get; set; }
			public string DrLName { get; set; }
			public string SpecialityName { get; set; }
		}
	}
}
