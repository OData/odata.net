namespace Microsoft.Epm
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            new HttpRequestHandler().Listen();
        }
    }
}