//---------------------------------------------------------------------
// <copyright file="ClrNullableDataTypesFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Fixup for changing data type nullability to be consistent with the CLR semantics used in Astoria providers
    /// </summary>
    public class ClrNullableDataTypesFixup : IEntityModelFixup
    {
        /// <summary>
        /// Changes the all complex properties to be nullable and all primitive types to be nullable if the clr allows it
        /// </summary>
        /// <param name="model">The model to fixup</param>
        public void Fixup(EntityModelSchema model)
        {
            this.FixupProperties(model.ComplexTypes.SelectMany(c => c.Properties).Union(model.EntityTypes.SelectMany(e => e.Properties.Where(p => p.IsPrimaryKey == false))));
        }

        private PrimitiveDataType ResolvePrimitiveType(PrimitiveDataType dataTypeSpecification)
        {
            if (dataTypeSpecification.IsNullable)
            {
                return dataTypeSpecification;
            }

            var facet = dataTypeSpecification.GetFacet<PrimitiveClrTypeFacet>();
            if (facet != null)
            {
                var type = facet.Value;
                if (type.IsClass() || (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    return dataTypeSpecification.Nullable();
                }
            }

            return dataTypeSpecification;
        }

        private void FixupProperties(IEnumerable<MemberProperty> properties)
        {
            foreach (MemberProperty prop in properties)
            {
                // resolve the type recursively
                var resolvedType = this.FixupType(prop.PropertyType);
                if (!object.ReferenceEquals(resolvedType, prop.PropertyType))
                {
                    // if the type changed, save the old type and update the property
                    prop.Annotations.Add(new TypeSpecificationAnnotation(prop.PropertyType));
                    prop.PropertyType = resolvedType;
                }
            }
        }

        private ComplexDataType ResolveComplexType(ComplexDataType dataTypeSpecification)
        {
            if (dataTypeSpecification.IsNullable)
            {
                return dataTypeSpecification;
            }
            else
            {
                return dataTypeSpecification.Nullable();
            }
        }

        private DataType FixupType(DataType dataType)
        {
            var complexDataType = dataType as ComplexDataType;
            if (complexDataType != null)
            {
                return this.ResolveComplexType(complexDataType);
            }

            var primitiveDataType = dataType as PrimitiveDataType;
            if (primitiveDataType != null)
            {
                var streamDataType = primitiveDataType as StreamDataType;
                if (streamDataType == null)
                {
                    return this.ResolvePrimitiveType(primitiveDataType);
                }
            }

            // EXPLICITLY DO NOT MODIFY TYPES WITHIN A COLLECTION, AS THEY ARE NOT NULLABLE
            return dataType;
        }
    }
}