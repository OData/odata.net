//---------------------------------------------------------------------
// <copyright file="ICombinatorialEngine.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Contract for combinatorial engines
    /// </summary>
    public interface ICombinatorialEngine
    {
        /// <summary>
        /// List of dimensions
        /// </summary>
        IEnumerable<CombinatorialDimension> Dimensions { get; }

        /// <summary>
        /// Returns the current values of all the dimensions, returns null if no combination is active
        /// </summary>
        Dictionary<string, object> CurrentDimensionValues { get; }

        /// <summary>
        /// Moves the engine to the next combination.
        /// </summary>
        /// <returns>True if there was another combination to return, false if no more combinations are available.</returns>
        bool NextCombination();
    }
}
