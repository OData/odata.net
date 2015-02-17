//---------------------------------------------------------------------
// <copyright file="AcceptableContentTypeSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;

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
        /// <param name="odataMaxVersion">OData-MaxVersion as specified by the request or determined from our logic.</param>
        /// <returns>A comma-separated list of client-supported MIME Accept types.</returns>
        string GetFormat(string dollarFormatValue, string acceptHeaderValue, Version odataMaxVersion);
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
        /// <param name="odataMaxVersion">OData-MaxVersion as specified by the request or determined from our logic.</param>
        /// <returns>A comma-separated list of client-supported MIME Accept types.</returns>
        public string GetFormat(string dollarFormatValue, string acceptHeaderValue, Version odataMaxVersion)
        {
            if (dollarFormatValue != null)
            {
                // V3+
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

            return acceptHeaderValue;
        }
    }
}