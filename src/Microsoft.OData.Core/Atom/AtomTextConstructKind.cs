//---------------------------------------------------------------------
// <copyright file="AtomTextConstructKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Enumeration for classifying the different kinds of text content in ATOM metadata.
    /// </summary>
    public enum AtomTextConstructKind
    {
        /// <summary>Plain text.</summary>
        Text = 0,

        /// <summary>Html text.</summary>
        Html = 1,

        /// <summary>XHtml text.</summary>
        Xhtml = 2,
    }
}
