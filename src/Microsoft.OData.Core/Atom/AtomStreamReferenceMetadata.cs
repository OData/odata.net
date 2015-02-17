//---------------------------------------------------------------------
// <copyright file="AtomStreamReferenceMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Atom metadata for stream reference values.
    /// </summary>
    public sealed class AtomStreamReferenceMetadata : ODataAnnotatable
    {
        /// <summary>Gets or sets an Atom link metadata for the self link.</summary>
        /// <returns>An Atom link metadata for the self link.</returns>
        public AtomLinkMetadata SelfLink
        {
            get;
            set;
        }

        /// <summary>Gets or sets an Atom link metadata for the edit link.</summary>
        /// <returns>An Atom link metadata for the edit link.</returns>
        public AtomLinkMetadata EditLink
        {
            get;
            set;
        }
    }
}
