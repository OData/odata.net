//---------------------------------------------------------------------
// <copyright file="IXmlToPayloadElementConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for converting an XML to a reach payload element representation.
    /// </summary>
    [ImplementationSelector("XmlToPayloadElementConverter", DefaultImplementation = "Default", HelpText = "The converter from an XML to a reach payload element representation.")] 
    public interface IXmlToPayloadElementConverter
    {
        /// <summary>
        /// Converts the given XML element into a rich payload element representation.
        /// </summary>
        /// <param name="element">XML element to convert.</param>
        /// <returns>A payload element representing the given element.</returns>
        ODataPayloadElement ConvertToPayloadElement(XElement element);
    }
}
