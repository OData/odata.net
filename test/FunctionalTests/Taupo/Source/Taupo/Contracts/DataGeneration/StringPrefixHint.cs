//---------------------------------------------------------------------
// <copyright file="StringPrefixHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a hint to put certain prefix on all generated strings.
    /// </summary>
    public sealed class StringPrefixHint : DataGenerationHint<string>
    {
        /// <summary>
        /// Initializes a new instance of the StringPrefixHint class.
        /// </summary>
        /// <param name="value">The common prefix.</param>
        internal StringPrefixHint(string value)
            : base(value)
        {
        }
    }
}
