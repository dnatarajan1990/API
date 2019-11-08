namespace RA.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static bool IsNotEmpty(this string source)
        {
            return !string.IsNullOrEmpty(source);
        }

        public static string Quote(this string source)
        {
            return "\"" + source + "\"";
        }
        public static string FixPathSlash(this string source)
        {
            if (source == null || source.Length == 0) return source;
            if (source.EndsWith("/")) source = source.Substring(0, source.Length - 1);
            if (!source.StartsWith("/") && !source.StartsWith("http")) source = "/"+source;
            return source;
        }
    }
}
