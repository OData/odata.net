//---------------------------------------------------------------------
// <copyright file="EnumMembersOnlyHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents Hint for generating only enum member values.
    /// </summary>
    public sealed class EnumMembersOnlyHint : SingletonDataGenerationHint
    {
        private static EnumMembersOnlyHint instance = new EnumMembersOnlyHint();

        /// <summary>
        /// Prevents a default instance of the EnumMembersOnlyHint class from being created.
        /// </summary>
        private EnumMembersOnlyHint()
        {
        }

        /// <summary>
        /// Gets the sole instance of the EnumMembersOnlyHint.
        /// </summary>
        /// <value>The EnumMembersOnlyHint.</value>
        internal static EnumMembersOnlyHint Instance
        {
            get { return instance; }
        }
    }
}
