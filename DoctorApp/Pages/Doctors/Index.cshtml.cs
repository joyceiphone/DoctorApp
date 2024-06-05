using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Authorization;
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
								  where t1.IsActive
								  select new JoinedResultModel
								  {
									  Id = t1.Id,
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
										  where t1.IsActive
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
										  where t1.SpecialityID == selectedId & t1.IsActive
										  select new JoinedResultModel
										  {
											  Id = t1.Id,
											  DrFName = t1.DrFName,
											  DrLName = t1.DrLName,
											  SpecialityName = t2.SpecialityName
										  }).ToListAsync();
				}
			}
			if (command == "PrintCheckedItems")
			{
				JoinedResult = await (from t1 in _context.Doctors
									  join t2 in _context.Specialties
									  on t1.SpecialityID equals t2.Id
									  where t1.IsActive
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

					var checkedIds = Request.Form["CheckedItems"].ToList();

					// Retrieve the details of the checked items from the database
					var checkedItems = await (from t1 in _context.Doctors
											  join t2 in _context.Specialties on t1.SpecialityID equals t2.Id
											  join t3 in _context.Addresses on t1.Id equals t3.DoctorId into addresses
											  where checkedIds.Contains(t1.Id.ToString()) & t1.IsActive
											  select new JoinedResultModel
											  {
												  Id = t1.Id,
												  DrFName = t1.DrFName,
												  DrLName = t1.DrLName,
												  SpecialityName = t2.SpecialityName,
												  Addresses = addresses.Any() ? 
												  addresses.Where(a => a.IsActive).ToList() : null,
											  }).ToListAsync();

					foreach (var item in checkedItems)
					{
						// Create an empty page in this document.
						var page = document.AddPage();
						//page.Size = PageSize.Letter;

						// Get an XGraphics object for drawing on this page.
						var gfx = XGraphics.FromPdfPage(page);

						XPen lineBlack = new XPen(XColors.Black, 2);
						// Define the starting and ending points of the line
						double startX = 50; // X-coordinate of the starting point
						double endX = page.Width - 50; // X-coordinate of the ending point
						double y = 40; // Y-coordinate of the line

						// Draw the horizontal line
						gfx.DrawLine(lineBlack, startX, y, endX, y);

						var height = page.Height;

						var font = new XFont("Times New Roman", 12, XFontStyleEx.Bold);
						y += 40;

						gfx.DrawString($"Specialty: {item.SpecialityName}", font, XBrushes.Black, new XPoint(50, y));
						y += 40;
						gfx.DrawString($"{item.DrFName} {item.DrLName} MD", font, XBrushes.Black, new XPoint(50, y));
						y += 40;

						if(item.Addresses != null )
						{
							foreach (var address in item.Addresses)
							{
								gfx.DrawString($"{address.Street1} {address.Street2} {address.City} {address.State} {address.ZipCode} {address.TelAddress}", font, XBrushes.Black, new XPoint(50, y));
								y += 40;
							}
						}
					};

					string tempPath = Path.Combine(Path.GetTempPath(), "testPdfSharp.pdf");
					document.Save(tempPath);

					Console.WriteLine("PDF saved to: " + tempPath);

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
			public List<Address> Addresses { get; set; }
		}
	}
}
