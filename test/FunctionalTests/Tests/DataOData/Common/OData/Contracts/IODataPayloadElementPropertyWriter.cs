//---------------------------------------------------------------------
// <copyright file="IODataPayloadElementPropertyWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    #region Namespaces
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Contract for property element driven OData message writer
    /// </summary>
    [ImplementationSelector("IODataPayloadElementPropertyWriter", DefaultImplementation = "ODataPayloadElementPropertyWriter", HelpText = "Writer that converts and writes a property element using an ODataMessageWriter.")]
    public interface IODataPayloadElementPropertyWriter
    {
        /// <summary>
        /// Writes the property element argument to the specified stream.
        /// </summary>
        /// <param name="writer">The writer to write the property element to.</param>
        /// <param name="element">The element to write.</param>
        void WriteProperty(ODataMessageWriter writer, ODataPayloadElement element);
    }
}
