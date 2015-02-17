//---------------------------------------------------------------------
// <copyright file="NoRandomValuesHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a hint to not generate any random values and use only interesting values provided by other hints.
    /// </summary>
    public sealed class NoRandomValuesHint : SingletonDataGenerationHint
    {
        private static NoRandomValuesHint instance = new NoRandomValuesHint();
        
        /// <summary>
        /// Prevents a default instance of the NoRandomValuesHint class from being created.
        /// </summary>
        private NoRandomValuesHint()
        {
        }

        /// <summary>
        /// Gets the sole instance of the NoRandomValuesHint.
        /// </summary>
        /// <value>The NoRandomValuesHint.</value>
        internal static NoRandomValuesHint Instance
        {
            get { return instance; }
        }
    }
}
