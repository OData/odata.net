//---------------------------------------------------------------------
// <copyright file="ODataTypeInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Metadata
{
    #region Namespaces.

    using System.Collections.Concurrent;
    using System.Reflection;

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
