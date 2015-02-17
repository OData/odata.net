//---------------------------------------------------------------------
// <copyright file="AllNullsHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a hint to generate all null values.
    /// </summary>
    public sealed class AllNullsHint : SingletonDataGenerationHint
    {
        private static AllNullsHint instance = new AllNullsHint();

        /// <summary>
        /// Prevents a default instance of the AllNullsHint class from being created.
        /// </summary>
        private AllNullsHint()
        {
        }

        /// <summary>
        /// Gets the sole instance of the AllNullsHint.
        /// </summary>
        /// <value>The AllNullsHint.</value>
        internal static AllNullsHint Instance
        {
            get { return instance; }
        }
    }
}
