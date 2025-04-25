namespace NewStuff.Odata.Headers
{
    public abstract class FormatRequestHeader //// TODO i think you just made up this name; use a better one when it comes along
    {
        private FormatRequestHeader()
        {
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
        /// </summary>
        public sealed class Json : FormatRequestHeader
        {
            public Json(NewStuff.Odata.Headers.Json.FormatRequestHeader formatRequestHeader)
            {
                FormatRequestHeader = formatRequestHeader;
            }

            public NewStuff.Odata.Headers.Json.FormatRequestHeader FormatRequestHeader { get; }
        }

        //// TODO i don't think other formats are allowed by the standard at the moment; you should figure out how custom formats get supported and add that derived type, though
    }
}
