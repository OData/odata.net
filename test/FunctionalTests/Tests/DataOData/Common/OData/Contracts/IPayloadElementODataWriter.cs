//---------------------------------------------------------------------
// <copyright file="IPayloadElementODataWriter.cs" company="Microsoft">
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
    /// Contract for payload element driven OData writer
    /// </summary>
    [ImplementationSelector("IPayloadElementODataWriter", DefaultImplementation = "PayloadElementODataWriter", HelpText = "Writer that converts and writes a payload element using an ODataWriter.")]
    public interface IPayloadElementODataWriter
    {
        /// <summary>
        /// Writes the payload element argument to the specified stream.
        /// </summary>
        /// <param name="writer">The writer to write the payload element to.</param>
        /// <param name="element">The element to write.</param>
        void WritePayload(ODataWriter writer, ODataPayloadElement element);
    }
}
