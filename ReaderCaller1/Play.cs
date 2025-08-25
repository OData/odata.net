namespace ReaderCaller1
{
    using System.Threading.Tasks;

    using Odata;

    public static class Play
    {
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
    }
}
