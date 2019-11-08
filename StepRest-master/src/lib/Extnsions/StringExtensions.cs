namespace StepRest.Extensions
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

        public static string SubStringTo(this string source, string to)
        {
            return source.Substring(source.IndexOf(to)+1);
        }
        public static string SubStringToLast(this string source, string to)
        {
            return source.Substring(source.LastIndexOf(to)+1);
        }
    }
}
