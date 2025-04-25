namespace NewStuff.Odata.Headers
{
    public abstract class Header
    {
        private Header()
        {
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_ProtocolVersioning
        /// </summary>
        public sealed class OdataVersion : Header
        {
            public OdataVersion(NewStuff.Odata.Headers.OdataVersion header)
            {
                Header = header;
            }

            public NewStuff.Odata.Headers.OdataVersion Header { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_ProtocolVersioning
        /// </summary>
        public sealed class OdataMaxVersion : Header
        {
            public OdataMaxVersion(NewStuff.Odata.Headers.OdataMaxVersion header)
            {
                Header = header;
            }

            public NewStuff.Odata.Headers.OdataMaxVersion Header { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_HeaderFieldExtensibility
        /// 
        /// The OData standard calls any header not used by OData a "custom" header, even headers defined by HTTP that are simply not directly leveraged by OData.
        /// </summary>
        public sealed class Custom : Header
        {
            public Custom(CustomHeader header)
            {
                Header = header;
            }

            public CustomHeader Header { get; }
        }
    }
}
