namespace Root
{
    using Sprache;
    using System.Linq;

    /// <summary>
    /// TODO put this in the right folder and namespace
    /// </summary>
    public static class Parser
    {
        public static Parser<T> None<T>(string errorMessage)
        {
            return input => Result.Failure<T>(input, errorMessage, Enumerable.Empty<string>());
        }
    }
}
