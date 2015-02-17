//---------------------------------------------------------------------
// <copyright file="ODataAtomDeserializerExpandedNavigationLinkContent.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Possible content types of expanded navigation link in ATOM.
    /// </summary>
    internal enum ODataAtomDeserializerExpandedNavigationLinkContent
    {
        /// <summary>No content found, no m:inline.</summary>
        None,

        /// <summary>Empty content found, m:inline without anything in it. Usually represents null entry.</summary>
        Empty,

        /// <summary>Expanded entry found.</summary>
        Entry,

        /// <summary>Expanded feed found.</summary>
        Feed
    }
}
