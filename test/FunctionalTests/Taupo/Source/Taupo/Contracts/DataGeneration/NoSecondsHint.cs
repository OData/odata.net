//---------------------------------------------------------------------
// <copyright file="NoSecondsHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a hint to not generate seconds part of the time.
    /// </summary>
    public sealed class NoSecondsHint : SingletonDataGenerationHint
    {
        private static NoSecondsHint instance = new NoSecondsHint();

        /// <summary>
        /// Prevents a default instance of the NoSecondsHint class from being created.
        /// </summary>
        private NoSecondsHint()
        {
        }

        /// <summary>
        /// Gets the sole instance of the NoSecondsHint.
        /// </summary>
        /// <value>The NoSecondsHint.</value>
        internal static NoSecondsHint Instance
        {
            get { return instance; }
        }
    }
}
