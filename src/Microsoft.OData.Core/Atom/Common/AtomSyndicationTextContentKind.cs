//---------------------------------------------------------------------
// <copyright file="AtomSyndicationTextContentKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    /// <summary>Enumeration used to identify text content of syndication item. </summary>
    public enum AtomSyndicationTextContentKind
    {
        /// <summary>
        /// Plaintext
        /// </summary>
        Plaintext,

        /// <summary>
        /// HTML
        /// </summary>
        Html,

        /// <summary>
        /// XHTML
        /// </summary>
        Xhtml
    }
}
