//---------------------------------------------------------------------
// <copyright file="CollectionMaxCountHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents maximum count for a collection.
    /// </summary>
    public sealed class CollectionMaxCountHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the CollectionMaxCountHint class.
        /// </summary>
        /// <param name="maxCount">Maximum count.</param>
        internal CollectionMaxCountHint(int maxCount)
            : base(maxCount)
        {
        }
    }
}
