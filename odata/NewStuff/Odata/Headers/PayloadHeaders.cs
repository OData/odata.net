namespace NewStuff.Odata.Headers
{
    using System.Collections.Generic;

    //// TODO you just assumed this was the implementation of this class; was there anything in the standard that deviated from it?
    public sealed class PayloadHeaders
    {
        public PayloadHeaders(IEnumerable<Header> headers)
        {
            Headers = headers;
        }

        public IEnumerable<Header> Headers { get; }
    }
}
