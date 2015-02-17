//---------------------------------------------------------------------
// <copyright file="ODataEntityReferenceLink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Represents an entity reference link (the result of a $link query).
    /// </summary>
    [DebuggerDisplay("{Url.OriginalString}")]
    public sealed class ODataEntityReferenceLink : ODataItem
    {
        /// <summary>Gets or sets the URI representing the URL of the referenced entity.</summary>
        /// <returns>The URI representing the URL of the referenced entity.</returns>
        /// <remarks>This URL should be usable to retrieve or modify the referenced entity.</remarks>
        public Uri Url
        {
            get;
            set;
        }
    }
}
