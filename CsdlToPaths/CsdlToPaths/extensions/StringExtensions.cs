






public static class StringExtensions
{
    public static string SeparatedBy(this IEnumerable<string> items, string separator)
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            sb.Append(separator);
            sb.Append(item);
        }
        return sb.ToString();
    }


    public static string Capitalize(this string word)
        => word[0..1].ToUpper() + word[1..];

}