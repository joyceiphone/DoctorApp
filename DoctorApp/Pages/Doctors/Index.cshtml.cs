using System.IO;
using DoctorApp.Data;
using DoctorApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.UniversalAccessibility.Drawing;


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

		public List<ReferralLetter> ReferralLetters { get; set; }

		[BindProperty]
		public int selectedId { get; set; }

		[BindProperty]
		public List<string> CheckedItems { get; set; }

		[BindProperty]
		public string PtAccNum { get; set; }

		List<Dictionary<string, string>> titleAddresses = new List<Dictionary<string, string>>
		{
			new Dictionary<string, string>
			{
				{ "leftText", "99 Elizabeth Street, 2FL" },
				{ "rightText", "42-70 Kissena BLVD" }
			},
			new Dictionary<string, string>
			{
				{ "leftText", "New York, NY, 10013" },
				{ "rightText", "Flushing, NY 11355" }
			},
			new Dictionary<string, string>
			{
				{ "leftText", "Tel: 212-227-8837" },
				{ "rightText", "Tel: 718-888-9838" }
			},
			new Dictionary<string, string>
			{
				{ "leftText", "Fax: 212-227-4651" },
				{ "rightText", "Fax: 212-227-4651" }
			}
		};

		public async Task OnGetAsync()
		{

			Options = await _context.Specialties.Where(s=>s.IsActive).Select(a =>
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
			Options = await _context.Specialties.Where(s=>s.IsActive).Select(a =>
								  new SelectListItem
								  {
									  Value = a.Id.ToString(),
									  Text = a.SpecialityName
								  }).ToListAsync();
			Options.Add(new SelectListItem { Value = "0", Text = "Show All" });

			if (ReferralLetters == null)
			{
				ReferralLetters = new List<ReferralLetter>();
			}

			if (command == "FilterAction")
			{
				// Logic for Action 1

				ModelState.Remove(nameof(PtAccNum));

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
					if (PtAccNum.Trim() == "" || PtAccNum == null)
					{
						ModelState.AddModelError("PtAccNum", "PtAccNum is required");
						return Page();
					}

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

					string destinationFileName = Path.GetRandomFileName();
					destinationFileName = Path.ChangeExtension(destinationFileName, "pdf");
					destinationFileName = DateTime.Now.ToString("yyMMddHHmmssfff") + destinationFileName;

					foreach (var item in checkedItems)
					{
						var newReferralLetter = new ReferralLetter { DoctorID = item.Id };
						newReferralLetter.DoctorID = item.Id;
						newReferralLetter.FileName = destinationFileName;
						newReferralLetter.PtAccNumber = PtAccNum.Trim();

						ReferralLetters.Add(newReferralLetter);

						// Create an empty page in this document.
						var page = document.AddPage();
						//page.Size = PageSize.Letter;

						// Get an XGraphics object for drawing on this page.
						var gfx = XGraphics.FromPdfPage(page);

						XPen lineBlack = new XPen(XColors.Black, 2);
						// Define the starting and ending points of the line

						string title = "ADVANCED EYE PHYSICIAN PLLC";
						var titleFont = new XFont("Times New Roman", 30, XFontStyleEx.Bold);
						double startX = 60; // X-coordinate of the starting point
						double endX = 60 + gfx.MeasureString(title, titleFont).Width; // X-coordinate of the ending point
						double y = 60; // Y-coordinate of the line

						gfx.DrawString(title, titleFont, XBrushes.Black, new XPoint(60, y));

						y += 2;

						// Draw the horizontal line
						gfx.DrawLine(lineBlack, startX, y, endX, y);

						y += 18;

						var height = page.Height;

						var font = new XFont("Times New Roman", 12, XFontStyleEx.Bold);

						XSize rightTextSize = gfx.MeasureString(titleAddresses[0]["rightText"], font);

						foreach (var titleAddress in titleAddresses)
						{
							string leftText = titleAddress["leftText"];
							XSize leftTextSize = gfx.MeasureString(leftText, font);
							gfx.DrawString(leftText, font, XBrushes.Black, new XPoint(60, y));

							string rightText = titleAddress["rightText"];
							double rightTextX = endX - rightTextSize.Width;
							gfx.DrawString(rightText, font, XBrushes.Black, new XPoint(rightTextX, y));

							y += 18;
						}

						y += 40;

						var bodyFont = new XFont("Times New Roman", 16, XFontStyleEx.Bold);

						gfx.DrawString($"Specialty: {item.SpecialityName}", bodyFont, XBrushes.Black, new XPoint(60, y));
						y += 40;
						gfx.DrawString($"{item.DrFName} {item.DrLName} MD", bodyFont, XBrushes.Black, new XPoint(60, y));
						y += 40;

						if(item.Addresses != null )
						{
							foreach (var address in item.Addresses)
							{
								gfx.DrawString($"{address.Street1} {address.Street2} {address.City} {address.State} {address.ZipCode} {address.TelAddress}", bodyFont, XBrushes.Black, new XPoint(60, y));
								y += 40;
							}
						}
					};

					string tempPath = Path.Combine(Path.GetTempPath(), "testPdfSharp.pdf");
					document.Save(tempPath);

					_context.ReferralLetters.AddRange(ReferralLetters);
					await _context.SaveChangesAsync();

					using (var stream = new MemoryStream())
					{
						document.Save(stream, false);
						stream.Position = 0;

						// Return the PDF file as a FileResult.
						return File(stream.ToArray(), "application/pdf", destinationFileName);
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
