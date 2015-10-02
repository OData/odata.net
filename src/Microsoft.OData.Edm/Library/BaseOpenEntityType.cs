//---------------------------------------------------------------------
// <copyright file="BaseOpenEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Library
{
    public sealed class BaseOpenEntityType : EdmStructuredType, IEdmEntityType
    {
        public BaseOpenEntityType(string name)
            :base(false, true, null)
        {
            this.Name = name;
        }

        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool HasStream
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get; private set;
        }

        public string Namespace
        {
            get
            {
                return string.Empty;
            }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public EdmTermKind TermKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override EdmTypeKind TypeKind
        {
            get
            {
                return EdmTypeKind.Entity;
            }
        }
    }

    public static class Extension
    {
        public static IEdmEntityType GetDynamicEntityType(Type type)
        {
            var edmType = new BaseOpenEntityType(type.Name);

            foreach (var prop in type.GetPublicProperties(instanceOnly: false))
            {
                var propEdmType = EdmCoreModel.Instance.FindDeclaredType(GetNonNullableType(prop.PropertyType).Name) as IEdmPrimitiveType;
                edmType.AddStructuralProperty(prop.Name, EdmCoreModel.Instance.GetPrimitive(propEdmType.PrimitiveKind, IsNullableType(prop.PropertyType)));
            }

            return edmType;
        }

        /// <summary>Checks whether the specified type is a generic nullable type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is nullable; false otherwise.</returns>
        internal static bool IsNullableType(Type type)
        {
            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>Gets a non-nullable version of the specified type.</summary>
        /// <param name="type">Type to get non-nullable version for.</param>
        /// <returns>
        /// <paramref name="type"/> if type is a reference type or a 
        /// non-nullable type; otherwise, the underlying value type.
        /// </returns>
        internal static Type GetNonNullableType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }

}
