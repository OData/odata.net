namespace Payloads
{
    public abstract class FormatRequestHeader
    {
        private FormatRequestHeader()
        {
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
        /// </summary>
        public sealed class Json : FormatRequestHeader
        {
            public Json(Payloads.Json.FormatRequestHeader formatRequestHeader)
            {
                FormatRequestHeader = formatRequestHeader;
            }

            public Payloads.Json.FormatRequestHeader FormatRequestHeader { get; }
        }

        //// TODO i don't think other formats are allowed by the standard at the moment; you should figure out how custom formats get supported and add that derived type, though
    }
}
