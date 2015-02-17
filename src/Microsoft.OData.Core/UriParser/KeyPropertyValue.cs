//---------------------------------------------------------------------
// <copyright file="KeyPropertyValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    #endregion Namespaces

    /// <summary>
    /// Class representing a single key property value in a key lookup.
    /// </summary>
    public sealed class KeyPropertyValue
    {
        /// <summary>
        /// Gets or sets the key property.
        /// </summary>
        public IEdmProperty KeyProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the key property.
        /// </summary>
        public SingleValueNode KeyValue
        {
            get;
            set;
        }
    }
}