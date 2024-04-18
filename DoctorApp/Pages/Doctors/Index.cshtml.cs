using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;
using PdfSharp.Fonts.OpenType;
using System.IO;


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
									  SpecialtyName = t2.SpecialityName,
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
											  SpecialtyName = t2.SpecialityName
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
											  SpecialtyName = t2.SpecialityName
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
										  SpecialtyName = t2.SpecialityName
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

					// Draw two lines with a red default pen.
					var width = page.Width;
					var height = page.Height;
					gfx.DrawLine(XPens.Red, 0, 0, width, height);
					gfx.DrawLine(XPens.Red, width, 0, 0, height);

					// Draw a circle with a red pen which is 1.5 point thick.
					var r = width / 5;
					gfx.DrawEllipse(new XPen(XColors.Red, 1.5), XBrushes.White, new XRect(width / 2 - r, height / 2 - r, 2 * r, 2 * r));

                    var font = new XFont("Times New Roman", 12, XFontStyleEx.Bold);
                    int y = 40;

					foreach (var item in CheckedItems)
					{
						gfx.DrawString("Hello, PDFsharp!", font, XBrushes.Black,
						new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
						y += 20;
					}

					// Save the document...
					//var filename = PdfFileUtility.GetTempPdfFullFileName("samples/HelloWorldSample");
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
			public string SpecialtyName { get; set; }
		}
	}
}
