//---------------------------------------------------------------------
// <copyright file="ODataServiceDocumentElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Abstract class representing an element (EntitySet, Singleton) in a service document.
    /// </summary>
    public abstract class ODataServiceDocumentElement : ODataAnnotatable
    {
        /// <summary>Gets or sets the URI representing the Unified Resource Locator (URL) to the element.</summary>
        /// <returns>The URI representing the Unified Resource Locator (URL) to the element.</returns>
        public Uri Url
        {
            get;
            set;
        }

        /// <summary>Gets or sets the name of the element; this is the entity set or singleton name in JSON and the HREF in Atom.</summary>
        /// <returns>The name of the element.</returns>
        /// <remarks>
        /// This property is required when reading and writing the JSON light format.
        /// If present in ATOM, it will be used to populate the title element.
        /// </remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the title of the element; this is the title in JSON.</summary>
        /// <returns>The title of the element.</returns>
        /// <remarks>
        /// This property is optional in JSON light format, containing a human-readable, language-dependent title for the object.
        /// The value is null if it is not present.
        /// </remarks>
        public string Title
        {
            get;
            set;
        }
    }
}