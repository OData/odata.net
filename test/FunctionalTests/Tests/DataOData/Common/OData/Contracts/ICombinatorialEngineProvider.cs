//---------------------------------------------------------------------
// <copyright file="ICombinatorialEngineProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for the combinatorial engine provider
    /// </summary>
    [ImplementationSelector("ICombinatorialEngineProvider", DefaultImplementation = "FullCombinatorialEngineProvider", HelpText = "Combinatorial Engine")]
    public interface ICombinatorialEngineProvider
    {
        /// <summary>
        /// Creates a new combinatorial engine with the specified dimensions.
        /// </summary>
        /// <param name="dimensions">Dimensions to create combinations from.</param>
        /// <returns>The new combinatorial engine to be used.</returns>
        ICombinatorialEngine CreateEngine(IEnumerable<CombinatorialDimension> dimensions);
    }
}
