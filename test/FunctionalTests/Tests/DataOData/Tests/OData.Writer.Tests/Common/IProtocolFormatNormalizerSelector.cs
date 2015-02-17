//---------------------------------------------------------------------
// <copyright file="IProtocolFormatNormalizerSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Provides a mechanism for getting format specific normalizer
    /// </summary>
    [ImplementationSelector("DefaultProtocolFormatNormalizerSelector", DefaultImplementation = "Default")]
    public interface IProtocolFormatNormalizerSelector
    {
        /// <summary>
        /// Gets the serialization strategy for the given content type and uri
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <returns>A serialization strategy</returns>
        IODataPayloadElementNormalizer GetNormalizer(string contentType);
    }
}
