//---------------------------------------------------------------------
// <copyright file="CollectionMinCountHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents minimum count for a collection.
    /// </summary>
    public sealed class CollectionMinCountHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the CollectionMinCountHint class.
        /// </summary>
        /// <param name="minCount">Minimum count.</param>
        internal CollectionMinCountHint(int minCount)
            : base(minCount)
        {
        }
    }
}
