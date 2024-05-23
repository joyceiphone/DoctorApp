using PdfSharp.Fonts;

namespace DoctorApp
{
    public class FileFontResolver : IFontResolver // FontResolverBase
    {
		private readonly string fontDirectory;

		public FileFontResolver()
		{
			fontDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts");
		}
		public string DefaultFontName => throw new NotImplementedException();

        public byte[] GetFont(string faceName)
        {
			string fontPath = Path.Combine(fontDirectory, faceName);
			if (!File.Exists(fontPath))
			{
				throw new FileNotFoundException($"Font file not found: {fontPath}");
			}

			return File.ReadAllBytes(fontPath);
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Times New Roman", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("Times New Roman.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("Times New Roman.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("Times New Roman.ttf");
                }
                else
                {
                    return new FontResolverInfo("Times New Roman.ttf");
                }
            }
            return null;
        }
    }
}
