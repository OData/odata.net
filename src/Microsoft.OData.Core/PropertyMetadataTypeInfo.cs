//---------------------------------------------------------------------
// <copyright file="PropertyMetadataTypeInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    /// <summary>
    /// The class to present a property type info which is resolved from metadata.
    /// </summary>
    internal class PropertyMetadataTypeInfo
    {
        public PropertyMetadataTypeInfo(string name, IEdmStructuredType owningType)
        {
            this.PropertyName = name;
            this.OwningType = owningType;
            this.EdmProperty = owningType == null ? null : owningType.FindProperty(name);
            this.IsUndeclaredProperty = this.EdmProperty == null;
            this.IsOpenProperty = (owningType != null && owningType.IsOpen && this.IsUndeclaredProperty);
            this.TypeReference = this.IsUndeclaredProperty ? null : this.EdmProperty.Type;
            this.FullName = this.TypeReference == null ? null : this.TypeReference.Definition.AsActualType().FullTypeName();
        }

        public string PropertyName { get; private set; }

        public IEdmProperty EdmProperty { get; private set; }

        public bool IsOpenProperty { get; private set; }

        public bool IsUndeclaredProperty { get; private set; }

        public IEdmStructuredType OwningType { get; private set; }

        public IEdmTypeReference TypeReference { get; private set; }

        public string FullName { get; private set; }
    }
}
