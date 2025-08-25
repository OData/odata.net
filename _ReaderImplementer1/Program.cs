namespace _ReaderImplementer1
{
    using System.Threading.Tasks;

    using Odata;

    internal class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("success!");
        }
    }

    public sealed class EntityIdHeaderValueReader<TNextReader> : IEntityIdHeaderValueReader<Version.V1, TNextReader>
    {
        private readonly IEntityIdHeaderValueReader<Version.V1, TNextReader> delegateReader;

        public EntityIdHeaderValueReader(IEntityIdHeaderValueReader<Version.V1, TNextReader> delegateReader)
        {
            this.delegateReader = delegateReader;
        }

        public Version.V1 Version { get; } = Odata.Version.V1.Instance;

        public ValueTask Read()
        {
            throw new System.NotImplementedException();
        }

        public IEntityIdReader<Version.V1, TNextReader> TryMoveNext(out bool moved)
        {
            throw new System.NotImplementedException();
        }
    }
}
