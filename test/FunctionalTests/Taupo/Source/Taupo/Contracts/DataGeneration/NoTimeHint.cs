//---------------------------------------------------------------------
// <copyright file="NoTimeHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a hint to not generate time part of a date.
    /// </summary>
    public sealed class NoTimeHint : SingletonDataGenerationHint
    {
        private static NoTimeHint instance = new NoTimeHint();

        /// <summary>
        /// Prevents a default instance of the NoTimeHint class from being created.
        /// </summary>
        private NoTimeHint()
        {
        }

        /// <summary>
        /// Gets the sole instance of the NoTimeHint.
        /// </summary>
        /// <value>The NoTimeHint.</value>
        internal static NoTimeHint Instance
        {
            get { return instance; }
        }
    }
}