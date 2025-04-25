namespace NewStuff.Odata.Headers
{
    public abstract class Header
    {
        private Header()
        {
        }

        public sealed class OdataVersion : Header
        {
            public OdataVersion(NewStuff.Odata.Headers.OdataVersion header)
            {
                Header = header;
            }

            public Odata.Headers.OdataVersion Header { get; }
        }

        //// TODO you need to add something here for "general headers" or "non-odata headers" or whatever
    }
}
