//---------------------------------------------------------------------
// <copyright file="ICombinatorialEngineProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.CombinatorialEngine
{
    using System.Collections.Generic;

    /// <summary>
    /// Contract for the combinatorial engine provider
    /// </summary>
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
