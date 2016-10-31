//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services
{
    /// <summary>
    /// Strategy for getting the content type based on $format and the accept header.
    /// </summary>
    internal interface IAcceptableContentTypeSelector
    {
        /// <summary>
        /// Gets a comma-separated list of client-supported MIME Accept types.
        /// If $format is found with a value we recognize, we must use the matching content type and ignore accept header.
        /// If $format is found but we don't recognize the value, return just that value (don't even concat to accept values). Later we'll probably throw.
        /// If no $format, use accept header value (might be a list).  Returning null indicates neither was set.
        /// </summary>
        /// <param name="dollarFormatValue">Value of $format or null.</param>
        /// <param name="acceptHeaderValue">Value of accept header or null.</param>
        /// <param name="maxDataServiceVersion">MaxDataServiceVersion as specified by the request or determined from our logic.</param>
        /// <returns>A comma-separated list of client-supported MIME Accept types.</returns>
        string GetFormat(string dollarFormatValue, string acceptHeaderValue, Version maxDataServiceVersion);
    }

    /// <summary>
    /// Strategy for getting the content type based on $format and the accept header for V3.
    /// </summary>
    internal class AcceptableContentTypeSelector : IAcceptableContentTypeSelector
    {
        /// <summary>
        /// Gets a comma-separated list of client-supported MIME Accept types.
        /// If $format is found with a value we recognize, we must use the matching content type and ignore accept header.
        /// If $format is found but we don't recognize the value, return just that value (don't even concat to accept values). Later we'll probably throw.
        /// If no $format, use accept header value (might be a list).  Returning null indicates neither was set.
        /// </summary>
        /// <param name="dollarFormatValue">Value of $format or null.</param>
        /// <param name="acceptHeaderValue">Value of accept header or null.</param>
        /// <param name="maxDataServiceVersion">MaxDataServiceVersion as specified by the request or determined from our logic.</param>
        /// <returns>A comma-separated list of client-supported MIME Accept types.</returns>
        public string GetFormat(string dollarFormatValue, string acceptHeaderValue, Version maxDataServiceVersion)
        {
            if (dollarFormatValue != null)
            {
                if (maxDataServiceVersion < new Version(3, 0))
                {
                    switch (dollarFormatValue)
                    {
                        case "xml":
                            return XmlConstants.MimeApplicationXml;
                        case "atom":
                            return XmlConstants.MimeApplicationAtom;
                        case "json":
                            return XmlConstants.MimeApplicationJson;
                        default:
                            return dollarFormatValue;
                    }
                }

                // V3+
                switch (dollarFormatValue)
                {
                    case "xml":
                        return XmlConstants.MimeApplicationXml;
                    case "atom":
                        return XmlConstants.MimeApplicationAtom;
                    case "json":
                        return XmlConstants.MimeApplicationJson;
                    case "verbosejson":
                        return XmlConstants.MimeApplicationJsonODataVerbose;
                    default:
                        return dollarFormatValue;
                }
            }

            return acceptHeaderValue;
        }
    }
}
