//---------------------------------------------------------------------
// <copyright file="PropertyValueTypeInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    /// <summary>
    /// The class to present the property type info which is resolved from property value.
    /// </summary>
    internal class PropertyValueTypeInfo
    {
        public PropertyValueTypeInfo(string typeName, IEdmTypeReference typeReference)
        {
            this.TypeName = typeName;
            this.TypeReference = typeReference;
            if (typeReference != null)
            {
                this.FullName = typeReference.FullName();
                this.IsPrimitive = typeReference.IsPrimitive();
                this.IsComplex = typeReference.IsComplex();
                this.PrimitiveTypeKind = this.IsPrimitive ? typeReference.AsPrimitive().PrimitiveKind() : EdmPrimitiveTypeKind.None;
            }
        }

        public IEdmTypeReference TypeReference { get; private set; }

        public string FullName { get; private set; }

        public bool IsPrimitive { get; private set; }

        public bool IsComplex { get; private set; }

        public string TypeName { get; private set; }

        public EdmPrimitiveTypeKind PrimitiveTypeKind { get; private set; }
    }
}
