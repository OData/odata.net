//---------------------------------------------------------------------
// <copyright file="NoNullsHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a hint to not generate null values.
    /// </summary>
    public sealed class NoNullsHint : SingletonDataGenerationHint
    {
        private static NoNullsHint instance = new NoNullsHint();
        
        /// <summary>
        /// Prevents a default instance of the NoNullsHint class from being created.
        /// </summary>
        private NoNullsHint()
        {
        }

        /// <summary>
        /// Gets the sole instance of the NoNullsHint.
        /// </summary>
        /// <value>The NoNullsHint.</value>
        internal static NoNullsHint Instance
        {
            get { return instance; }
        }
    }
}
