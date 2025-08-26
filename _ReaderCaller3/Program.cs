namespace _ReaderCaller3
{
    using System.Threading.Tasks;

    using Odata;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            await ReaderImplementer1().ConfigureAwait(false);
            await ReaderImplementer2().ConfigureAwait(false);
            await ReaderImplementer2Again().ConfigureAwait(false);
        }

        static async Task ReaderImplementer1()
        {
            var entityIdHeaderValueReader = new _ReaderImplementer1.EntityIdHeaderValueReader<object>(new ImplementationV1.EntityIdHeaderValueReader<object>());
            var nextReader = await Read(entityIdHeaderValueReader).ConfigureAwait(false);
            System.Console.WriteLine("success!");
        }

        static async Task ReaderImplementer2()
        {
            var entityIdHeaderValueReader = new _ReaderImplementer2.EntityIdHeaderValueReader<object>(new ImplementationV2.EntityIdHeaderValueReader<object>());
            var nextReader = await Read(entityIdHeaderValueReader).ConfigureAwait(false);
            System.Console.WriteLine("success!");
        }

        static async Task ReaderImplementer2Again()
        {
            var entityIdHeaderValueReader = new _ReaderImplementer2.EntityIdHeaderValueReader<object>(new ImplementationV2.EntityIdHeaderValueReader<object>());
            var nextReader = await Read2(entityIdHeaderValueReader).ConfigureAwait(false);
            System.Console.WriteLine("success!");
        }

        public static async Task<TNextReader> Read<TNextReader>(IEntityIdHeaderValueReader<Version.V1, TNextReader> entityIdHeaderValueReader)
        {
            IEntityIdReader<Version.V1, TNextReader> entityIdReader;
            while (!entityIdHeaderValueReader.TryMoveNext2(out entityIdReader))
            {
                await entityIdHeaderValueReader.Read().ConfigureAwait(false);
            }

            TNextReader next;
            while (!entityIdReader.TryMoveNext2(out next))
            {
                await entityIdReader.Read().ConfigureAwait(false);
            }

            return next;
        }

        public static async Task<TNextReader> Read2<TNextReader>(IEntityIdHeaderValueReader<Version.V2, TNextReader> entityIdHeaderValueReader)
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
