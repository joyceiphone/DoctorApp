namespace DoctorApp.Utilities
{
	public static class StringExtensions
	{
		public static string CapitalizeFirstLetter(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return char.ToUpper(value[0]) + value.Substring(1).ToLower();
		}

		public static string CapitalizeLetters(this string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return value.ToUpper();
		}
	}
}
