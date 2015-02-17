//---------------------------------------------------------------------
// <copyright file="EntityModelPrimitiveTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Resolves all primitive types in EntityModel by applying the specific data type resolver.
    /// </summary>
    [ImplementationName(typeof(IEntityModelPrimitiveTypeResolver), "Default")]
    public class EntityModelPrimitiveTypeResolver : IEntityModelPrimitiveTypeResolver
    {
        /// <summary>
        /// Initializes a new instance of the EntityModelPrimitiveTypeResolver class.
        /// </summary>
        public EntityModelPrimitiveTypeResolver()
        {
        }

        /// <summary>
        /// Resolves the provider types in a model.
        /// </summary>
        /// <param name="model">The model to resolve types for.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public void ResolveProviderTypes(EntityModelSchema model, IPrimitiveDataTypeResolver typeResolver)
        {
            foreach (EntityType entityType in model.EntityTypes)
            {
                this.FixupProperties(entityType.Properties, typeResolver);
                this.FixupGenericArguments(entityType, typeResolver);
            }

            foreach (ComplexType complexType in model.ComplexTypes)
            {
                this.FixupProperties(complexType.Properties, typeResolver);
            }

            foreach (Function function in model.Functions)
            {
                this.FixupParameters(function.Parameters, typeResolver);
                if (function.ReturnType != null)
                {
                    function.ReturnType = this.ResolvePrimitiveInDataType(function.ReturnType, typeResolver);
                }

                foreach (var returnTypeAnnotation in function.Annotations.OfType<StoredProcedureReturnTypeFunctionAnnotation>())
                {
                    returnTypeAnnotation.ReturnType = this.ResolvePrimitiveInDataType(returnTypeAnnotation.ReturnType, typeResolver);
                }
            }

            foreach (EntityContainer container in model.EntityContainers)
            {
                foreach (FunctionImport import in container.FunctionImports)
                {
                    this.FixupParameters(import.Parameters, typeResolver);
                    foreach (var returnType in import.ReturnTypes)
                    {
                        returnType.DataType = this.ResolvePrimitiveInDataType(returnType.DataType, typeResolver);
                    }
                }
            }
        }

        private DataType ResolvePrimitiveInDataType(DataType dataType, IPrimitiveDataTypeResolver typeResolver)
        {
            var primitiveDataType = dataType as PrimitiveDataType;
            if (primitiveDataType != null)
            {
                return typeResolver.ResolvePrimitiveType(primitiveDataType);
            }

            var collectionDataType = dataType as CollectionDataType;
            if (collectionDataType != null)
            {
                // using DataTypes.CollectionDataType here will lose nullability information
                return collectionDataType.WithElementDataType(this.ResolvePrimitiveInDataType(collectionDataType.ElementDataType, typeResolver));
            }

            var rowDataType = dataType as RowDataType;
            if (rowDataType != null)
            {
                this.FixupProperties(rowDataType.Definition.Properties, typeResolver);
            }

            return dataType;
        }

        private void FixupGenericArguments(EntityType entityType, IPrimitiveDataTypeResolver typeResolver)
        {
            var genericArgumentsAnnotation = entityType.Annotations.OfType<GenericArgumentsAnnotation>().SingleOrDefault();
            if (genericArgumentsAnnotation != null)
            {
                foreach (var genericArgument in genericArgumentsAnnotation.GenericArguments)
                {
                    genericArgument.DataType = this.ResolvePrimitiveInDataType(genericArgument.DataType, typeResolver);
                }
            }
        }

        private void FixupParameters(IEnumerable<FunctionParameter> parameters, IPrimitiveDataTypeResolver typeResolver)
        {
            foreach (FunctionParameter parameter in parameters)
            {
                // save the old parameter type
                parameter.Annotations.Add(new TypeSpecificationAnnotation(parameter.DataType));
                parameter.DataType = this.ResolvePrimitiveInDataType(parameter.DataType, typeResolver);
            }
        }

        private void FixupProperties(IEnumerable<MemberProperty> properties, IPrimitiveDataTypeResolver typeResolver)
        {
            foreach (MemberProperty prop in properties)
            {
                // resolve the type recursively
                var resolvedType = this.ResolvePrimitiveInDataType(prop.PropertyType, typeResolver);
                if (!object.ReferenceEquals(resolvedType, prop.PropertyType))
                {
                    // if the type changed, save the old type and update the property
                    prop.Annotations.Add(new TypeSpecificationAnnotation(prop.PropertyType));
                    prop.PropertyType = resolvedType;
                }
            }
        }
    }
}