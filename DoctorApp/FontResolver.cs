using PdfSharp.Fonts;

namespace DoctorApp
{
    public class FileFontResolver : IFontResolver // FontResolverBase
    {
        public string DefaultFontName => throw new NotImplementedException();

        public byte[] GetFont(string faceName)
        {
            using (var ms = new MemoryStream())
            {
                using (var fs = File.Open(faceName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Times New Roman", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("Fonts/Times New Roman.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("Fonts/Times New Roman.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("Fonts/Times New Roman.ttf");
                }
                else
                {
                    return new FontResolverInfo("Fonts/Times New Roman.ttf");
                }
            }
            return null;
        }
    }
}
