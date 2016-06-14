//---------------------------------------------------------------------
// <copyright file="PropertySerializationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;

namespace Microsoft.OData
{
    /// <summary>
    /// The class to hold all the info needed for a property in serialization.
    /// </summary>
    internal class PropertySerializationInfo
    {
        private bool isTopLevel;

        public PropertySerializationInfo(string name, IEdmStructuredType owningType)
        {
            this.PropertyName = name;
            this.IsTopLevel = false;
            this.MetadataType = new PropertyMetadataTypeInfo(name, owningType);
        }

        /// <summary>Name of current property.</summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// The type info resolved from property value.
        /// The value type info might change for a property in different ODataResource of an ODataResourceSet.
        /// </summary>
        public PropertyValueTypeInfo ValueType { get; set; }

        /// <summary>The type info resolved from metadata.</summary>
        public PropertyMetadataTypeInfo MetadataType { get; private set; }

        /// <summary>
        /// The value of '@odata.type' for current property, if the value is null, no type annotation needed.
        /// </summary>
        public string TypeNameToWrite { get; set; }

        /// <summary>The property name written in the wire.</summary>
        public string WireName { get; private set; }

        /// <summary>Whether the property is top level.</summary>
        public bool IsTopLevel
        {
            get
            {
                return isTopLevel;
            }

            set
            {
                isTopLevel = value;
                this.WireName = isTopLevel ? JsonLightConstants.ODataValuePropertyName : this.PropertyName;
            }
        }
    }
}
