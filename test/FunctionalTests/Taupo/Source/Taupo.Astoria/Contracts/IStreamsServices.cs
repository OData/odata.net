//---------------------------------------------------------------------
// <copyright file="IStreamsServices.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Collection of useful services for working with streams (both named and default)
    /// </summary>
    [ImplementationSelector("StreamsServices", DefaultImplementation = "Default", HelpText = "Utilities for working with streams")]
    public interface IStreamsServices
    {
        /// <summary>
        /// Creates and populates data for streams
        /// </summary>
        /// <param name="continuation">The continuation to indicate completion or failure on</param>
        void PopulateStreamsData(IAsyncContinuation continuation);
    }
}