namespace _ReaderCaller2
{
    using System.Threading.Tasks;

    using ImplementationV2;
    using Odata;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var entityIdHeaderValueReader = new EntityIdHeaderValueReader<object>();
            var nextReader = await Read(entityIdHeaderValueReader).ConfigureAwait(false);
            System.Console.WriteLine("success!");
        }

        public static async Task<TNextReader> Read<TNextReader>(IEntityIdHeaderValueReader<Version.V2, TNextReader> entityIdHeaderValueReader)
        {
            IEntityIdStartReader<Version.V2, TNextReader> entityIdStartReader;
            while (!entityIdHeaderValueReader.Version.TryMoveNext(entityIdHeaderValueReader, out entityIdStartReader))
            {
                await entityIdHeaderValueReader.Read().ConfigureAwait(false);
            }

            IIriSchemeReader<Version.V2, TNextReader> iriSchemeReader;
            while (!entityIdStartReader.TryMoveNext2(out iriSchemeReader))
            {
                await entityIdStartReader.Read().ConfigureAwait(false);
            }

            TNextReader nextReader;
            while (!iriSchemeReader.TryMoveNext2(out nextReader))
            {
                await iriSchemeReader.Read().ConfigureAwait(false);
            }

            return nextReader;
        }
    }
}
