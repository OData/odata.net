//---------------------------------------------------------------------
// <copyright file="ClientTypeUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Metadata
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using c = Microsoft.OData.Client;
    using System.Collections.Concurrent;

    #endregion Namespaces.

    /// <summary>
    /// Detailed information of a Type.
    /// </summary>
    internal class ODataTypeInfo
    {
        /// <summary>
        /// Creates and instance of <see cref="ODataTypeInfo"/>
        /// </summary>
        public ODataTypeInfo()
        {
            ClientDefinedNameDict = new ConcurrentDictionary<string, string>();
            ClientPropertyInfoDict = new ConcurrentDictionary<string, PropertyInfo>();            
        }

        /// <summary>
        /// Property Info array for the type
        /// </summary>
        public PropertyInfo[] KeyProperties { get; set; }

        /// <summary>
        /// See if the type has properties
        /// </summary>
        public bool? HasProperties { get; set; }

        /// <summary>
        /// Sertver defined type name
        /// </summary>
        public string ServerDefinedTypeName { get; set; }


        /// <summary>
        /// Sertver defined type full name
        /// </summary>
        public string ServerDefinedTypeFullName { get; set; }

        /// <summary>
        /// Concurrent Dictionary cache to save ClientDefinedName for the type and serverDefinedName
        /// </summary>
        public ConcurrentDictionary<string, string> ClientDefinedNameDict { get; set; }

        /// <summary>
        /// Concurrent Dictionary cache to save Propertyinfo for the type and serverDefinedName
        /// </summary>
        public ConcurrentDictionary<string, PropertyInfo> ClientPropertyInfoDict { get; set; }
    }
}
