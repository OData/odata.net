//---------------------------------------------------------------------
// <copyright file="EdmModelUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Edmlib;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with models.
    /// </summary>
    public static class EdmModelUtils
    {
        /// <summary>
        /// Finds all the entity containers in the specified and all referenced models.
        /// </summary>
        /// <returns>All entity containers in the <paramref name="model"/> and its references.</returns>
        public static IEnumerable<IEdmEntityContainer> EntityContainersAcrossModels(this IEdmModel model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            IEdmEntityContainer container = model.EntityContainer;
            if (container != null)
            {
                yield return container;
            }

            IEnumerable<IEdmModel> references = model.ReferencedModels;
            if (references != null)
            {
                foreach (var reference in references)
                {
                    foreach (IEdmEntityContainer referenceContainer in reference.EntityContainersAcrossModels())
                    {
                        yield return referenceContainer;
                    }
                }
            }
        }

        /// <summary>
        /// Resolves the specified entity model structured type and returns the Edm model type for it.
        /// </summary>
        /// <param name="model">The model to get the type from.</param>
        /// <param name="structuredType">The entity model structured type to resolve.</param>
        /// <returns>The resolved type for the specified <paramref name="structuredType"/>.</returns>
        public static IEdmStructuredType ResolveEntityModelSchemaStructuredType(IEdmModel model, NamedStructuralType structuredType)
        {
            DataType owningDataType = null;
            EntityType owningEntityType = structuredType as EntityType;
            if (owningEntityType != null)
            {
                owningDataType = DataTypes.EntityType.WithDefinition(owningEntityType);
            }

            ComplexType owningComplexType = structuredType as ComplexType;
            if (owningComplexType != null)
            {
                owningDataType = DataTypes.ComplexType.WithDefinition(owningComplexType);
            }

            if (owningDataType == null)
            {
                return null;
            }

            IEdmStructuredTypeReference structuredTypeReference = EdmModelUtils.ResolveEntityModelSchemaType(model, owningDataType) as IEdmStructuredTypeReference;
            return structuredTypeReference == null ? null : structuredTypeReference.StructuredDefinition();
        }

        /// <summary>
        /// Resolves the specified entity model schema type and returns the Edm model type for it.
        /// </summary>
        /// <param name="model">The model to get the type from.</param>
        /// <param name="schemaType">The entity model schema type to resolve.</param>
        /// <returns>The resolved type for the specified <paramref name="schemaType"/>.</returns>
        public static IEdmTypeReference ResolveEntityModelSchemaType(IEdmModel model, DataType schemaType)
        {
            if (schemaType == null)
            {
                return null;
            }

            PrimitiveDataType primitiveDataType = schemaType as PrimitiveDataType;
            if (primitiveDataType != null)
            {
                return GetPrimitiveTypeReference(primitiveDataType);
            }

            if (model == null)
            {
                return null;
            }

            EntityDataType entityDataType = schemaType as EntityDataType;
            if (entityDataType != null)
            {
                IEdmNamedElement edmType = model.FindType(entityDataType.Definition.FullName);
                ExceptionUtilities.Assert(
                    edmType != null,
                    "The expected entity type '{0}' was not found in the entity model for this test.",
                    entityDataType.Definition.FullName);

                IEdmEntityType entityType = edmType as IEdmEntityType;
                ExceptionUtilities.Assert(
                    entityType != null,
                    "The expected entity type '{0}' is not defined as entity type in the test's metadata.",
                    entityDataType.Definition.FullName);
                return entityType.ToTypeReference();
            }

            ComplexDataType complexDataType = schemaType as ComplexDataType;
            if (complexDataType != null)
            {
                return GetComplexType(model, complexDataType);
            }

            CollectionDataType collectionDataType = schemaType as CollectionDataType;
            if (collectionDataType != null)
            {
                DataType collectionElementType = collectionDataType.ElementDataType;
                PrimitiveDataType primitiveElementType = collectionElementType as PrimitiveDataType;
                if (primitiveElementType != null)
                {
                    IEdmPrimitiveTypeReference primitiveElementTypeReference = GetPrimitiveTypeReference(primitiveElementType);
                    return primitiveElementTypeReference.ToCollectionTypeReference();
                }

                ComplexDataType complexElementType = collectionElementType as ComplexDataType;
                if (complexElementType != null)
                {
                    IEdmComplexTypeReference complexElementTypeReference = GetComplexType(model, complexElementType);
                    return complexElementTypeReference.ToCollectionTypeReference();
                }

                EntityDataType entityElementType = collectionElementType as EntityDataType;
                if (entityElementType != null)
                {
                    IEdmEntityTypeReference entityElementTypeReference = GetEntityType(model, entityElementType);
                    return entityElementTypeReference.ToCollectionTypeReference();
                }

                throw new NotSupportedException("Collection types only support primitive, complex, and entity element types.");
            }

            StreamDataType streamType = schemaType as StreamDataType;
            if (streamType != null)
            {
                Type systemType = streamType.GetFacet<PrimitiveClrTypeFacet>().Value;
                ExceptionUtilities.Assert(systemType == typeof(Stream), "Expected the system type 'System.IO.Stream' for a stream reference property.");
                return MetadataUtils.GetPrimitiveTypeReference(systemType);
            }

            throw new NotImplementedException("Unrecognized schema type " + schemaType.GetType().Name + ".");
        }

        /// <summary>
        /// Resolves the specified entity model function import and returns the Edm model function import for it.
        /// </summary>
        /// <param name="model">The model to get the type from.</param>
        /// <param name="functionImport">The entity model function import to resolve.</param>
        /// <returns>The resolved function import for the specified <paramref name="functionImport"/>.</returns>
        public static IEdmOperationImport ResolveEntityModelSchemaFunctionImport(IEdmModel model, FunctionImport functionImport)
        {
            if (functionImport == null)
            {
                return null;
            }

            if (model == null)
            {
                return null;
            }

            IEdmEntityContainer edmEntityContainer = model.FindEntityContainer(functionImport.Container.FullName);
            if (edmEntityContainer == null)
            {
                ExceptionUtilities.Assert(
                    edmEntityContainer != null,
                    "The entity container '{0}' for function import '{1}' was not found in the model.",
                    functionImport.Container.FullName,
                    functionImport.Name);
            }

            IEnumerable<IEdmOperationImport> edmFunctionImports = edmEntityContainer.FindOperationImports(functionImport.Name);
            ExceptionUtilities.Assert(
                edmFunctionImports.Count() == 1,
                "There's either no or more than one function import with name '{0}' in the entity container '{1}'.",
                functionImport.Name,
                functionImport.Container.FullName);

            return edmFunctionImports.Single();
        }

        /// <summary>
        /// Gets the IEdmType for the given type name.
        /// </summary>
        /// <param name="fullTypeName">Name of type.</param>
        /// <returns>The IEdmType that corresponds to given typename.</returns>
        public static IEdmType GetPrimitiveTypeByName(string fullTypeName)
        {
            return EdmCoreModel.Instance.GetPrimitiveType(EdmCoreModel.Instance.GetPrimitiveTypeKind(fullTypeName));
        }

        /// <summary>
        /// Converts a primitive data type to type.
        /// </summary>
        /// <param name="primitiveType">The primitive data type to convert.</param>
        /// <returns>The corresponding primitive type.</returns>
        private static IEdmPrimitiveTypeReference GetPrimitiveTypeReference(PrimitiveDataType primitiveType)
        {
            Debug.Assert(primitiveType != null, "primitiveType != null");

            Type systemType = EntityModelUtils.GetPrimitiveClrType(primitiveType);

            // NOTE: if the primitiveType is not nullable but the type reference constructed from the CLR type is, 
            //       adjust the nullability if necessary.
            IEdmPrimitiveTypeReference primitiveTypeReference = MetadataUtils.GetPrimitiveTypeReference(systemType);
            if (primitiveType.IsNullable != primitiveTypeReference.IsNullable)
            {
                primitiveTypeReference = (IEdmPrimitiveTypeReference)primitiveTypeReference.Clone(primitiveType.IsNullable);
            }

            return primitiveTypeReference;
        }

        /// <summary>
        /// Converts a complex data type to a complex type.
        /// </summary>
        /// <param name="complexType">The complex data type to convert.</param>
        /// <returns>The corresponding complex type.</returns>
        private static IEdmComplexTypeReference GetComplexType(IEdmModel model, ComplexDataType complexDataType)
        {
            Debug.Assert(complexDataType != null, "complexDataType != null");

            IEdmSchemaType edmType = model.FindType(complexDataType.Definition.FullName);
            ExceptionUtilities.Assert(
                edmType != null,
                "The expected complex type '{0}' was not found in the entity model for this test.",
                complexDataType.Definition.FullName);

            IEdmComplexType complexType = edmType as IEdmComplexType;
            ExceptionUtilities.Assert(
                complexType != null,
                "The expected complex type '{0}' is not defined as complex type in the test's metadata.",
                complexDataType.Definition.FullName);
            return (IEdmComplexTypeReference)complexType.ToTypeReference(complexDataType.IsNullable);
        }

        /// <summary>
        /// Converts an entity data type to a entity type.
        /// </summary>
        /// <param name="entity">The entity data type to convert.</param>
        /// <returns>The corresponding entity type.</returns>
        private static IEdmEntityTypeReference GetEntityType(IEdmModel model, EntityDataType entityDataType)
        {
            Debug.Assert(entityDataType != null, "entityDataType != null");

            IEdmSchemaType edmType = model.FindType(entityDataType.Definition.FullName);
            ExceptionUtilities.Assert(
                edmType != null,
                "The expected entity type '{0}' was not found in the entity model for this test.",
                entityDataType.Definition.FullName);

            IEdmEntityType entityType = edmType as IEdmEntityType;
            ExceptionUtilities.Assert(
                entityType != null,
                "The expected entity type '{0}' is not defined as entity type in the test's metadata.",
                entityDataType.Definition.FullName);
            return (IEdmEntityTypeReference)entityType.ToTypeReference(entityDataType.IsNullable);
        }
    }
}
