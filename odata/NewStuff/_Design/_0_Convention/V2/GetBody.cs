namespace NewStuff._Design._0_Convention.Readers
{
    using System.IO;

    public sealed class GetBody
    {
        private GetBody()
        {
        }

        public Stream Data { get; } //// TODO there shouldn't be a body for a get request, but HTTP does allow it
    }
}
