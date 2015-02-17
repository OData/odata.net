//---------------------------------------------------------------------
// <copyright file="AnsiStringHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a hint to generate ANSI string.
    /// </summary>
    public sealed class AnsiStringHint : SingletonDataGenerationHint
    {
        private static AnsiStringHint instance = new AnsiStringHint();

        /// <summary>
        /// Prevents a default instance of the AnsiStringHint class from being created.
        /// </summary>
        private AnsiStringHint()
        {
        }

        /// <summary>
        /// Gets the sole instance of the AnsiStringHint.
        /// </summary>
        /// <value>The AnsiStringHint.</value>
        internal static AnsiStringHint Instance
        {
            get { return instance; }
        }
    }
}
