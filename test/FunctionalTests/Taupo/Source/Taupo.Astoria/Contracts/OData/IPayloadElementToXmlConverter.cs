//---------------------------------------------------------------------
// <copyright file="IPayloadElementToXmlConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for converting a reach payload element representation into XML representation.
    /// </summary>
    [ImplementationSelector("PayloadElementToXmlConverter", DefaultImplementation = "Default", HelpText = "The converter from a reach payload element representation to an XML representation.")]    
    public interface IPayloadElementToXmlConverter
    {
        /// <summary>
        /// Converts the given payload element into an XML representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>
        /// <returns>The XML representation of the payload.</returns>
        XElement ConvertToXml(ODataPayloadElement rootElement);
    }
}
