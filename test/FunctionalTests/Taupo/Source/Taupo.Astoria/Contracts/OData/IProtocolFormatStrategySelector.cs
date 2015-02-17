//---------------------------------------------------------------------
// <copyright file="IProtocolFormatStrategySelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Provides a mechanism for getting wire format representations for a given mime type
    /// </summary>
    [ImplementationSelector("ProtocolFormatStrategySelector", 
        DefaultImplementation = "Default",
        HelpText = "The selector used when finding the wire format for a given content type")]
    public interface IProtocolFormatStrategySelector
    {
        /// <summary>
        /// Gets the serialization strategy for the given content type and uri
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <param name="uri">The request uri</param>
        /// <returns>A serialization strategy</returns>
        IProtocolFormatStrategy GetStrategy(string contentType, ODataUri uri);
    }
}
