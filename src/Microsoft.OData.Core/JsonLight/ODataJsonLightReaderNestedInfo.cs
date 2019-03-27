﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderNestedInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    internal abstract class ODataJsonLightReaderNestedInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nestedProperty">The nested property for which the nested resource info will be reported.</param>
        internal ODataJsonLightReaderNestedInfo(IEdmProperty nestedProperty)
        {
            this.NestedProperty = nestedProperty;
        }

        /// <summary>
        /// The Edm property for which the nested resource info will be reported.
        /// </summary>
        internal IEdmProperty NestedProperty { get; private set; }
    }
}