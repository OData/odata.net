//---------------------------------------------------------------------
// <copyright file="EdmToTaupoModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Edmlib.Contracts;

    /// <summary>
    /// Converts a model from Edm term into Taupo term
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Converter needs coupling with a lot of classes")]
    [ImplementationName(typeof(IEdmToTaupoModelConverter), "Default")]
    public class EdmToTaupoModelConverter : IEdmToTaupoModelConverter
    {
        private IEdmModel edmModel;

        private AssociationRegistry associationRegistry;

        /// <summary>
        /// Gets the EDM model.
        /// </summary>
        protected IEdmModel EdmModel
        {
            get { return this.edmModel; }
        }

        /// <summary>
        /// Converts a model from Edm term into Taupo term
        /// </summary>
        /// <param name="edmModel">The input model in Edm term</param>
        /// <returns>The output model in Taupo term</returns>
        public EntityModelSchema ConvertToTaupoModel(IEdmModel edmModel)
        {
            this.edmModel = edmModel;
            this.associationRegistry = new AssociationRegistry();

            var taupoModel = new EntityModelSchema();

            foreach (var edmComplexType in edmModel.SchemaElements.OfType<IEdmComplexType>())
            {
                ComplexType taupoComplexType = this.ConvertToTaupoComplexType(edmComplexType);
                taupoModel.Add(taupoComplexType);
            }

            foreach (var edmEntityType in edmModel.SchemaElements.OfType<IEdmEntityType>())
            {
                EntityType taupoEntityType = this.ConvertToTaupoEntityType(edmEntityType);
                taupoModel.Add(taupoEntityType);

                // convert to Association using information inside Navigations
                foreach (var edmNavigationProperty in edmEntityType.DeclaredNavigationProperties())
                {
                    if (!this.associationRegistry.IsAssociationRegistered(edmNavigationProperty))
                    {
                        this.associationRegistry.RegisterAssociation(edmNavigationProperty);
                    }
                }
            }

            var edmEntityContainer = edmModel.EntityContainer;
            if (edmEntityContainer != null)
            {
                EntityContainer taupoEntityContainer = this.ConvertToTaupoEntityContainer(edmEntityContainer);
                taupoModel.Add(taupoEntityContainer);
            }

            foreach (var edmFunction in edmModel.SchemaElements.OfType<IEdmOperation>())
            {
                Function taupoFunction = this.ConvertToTaupoFunction(edmFunction);
                taupoModel.Add(taupoFunction);
            }

            foreach (var edmEnum in edmModel.SchemaElements.OfType<IEdmEnumType>())
            {
                EnumType taupoEnumType = this.ConvertToTaupoEnumType(edmEnum);
                taupoModel.Add(taupoEnumType);
            }

            return taupoModel.Resolve();
        }

        /// <summary>
        /// Convert annotations from Edm into Taupo
        /// </summary>
        /// <param name="edmAnnotatableItem">The edm item with Annotation</param>
        /// <param name="taupoAnnotatableItem">the taupo item with Annotation</param>
        /// <remarks>By default do nothing (because Annotation is *VERY* open). Please override in derived types to handle it properly.</remarks>
        protected virtual void ConvertAnnotationsIntoTaupo(IEdmElement edmAnnotatableItem, IAnnotatedItem taupoAnnotatableItem)
        {
        }

        private ComplexType ConvertToTaupoComplexType(IEdmComplexType edmComplexType)
        {
            // Taupo TODO: Abstract for ComplexType
            var taupoComplexType = new ComplexType(edmComplexType.Namespace, edmComplexType.Name);

            if (edmComplexType.BaseType != null)
            {
                taupoComplexType.BaseType = new ComplexTypeReference(edmComplexType.BaseComplexType().Namespace, edmComplexType.BaseComplexType().Name);
            }

            foreach (var edmProperty in edmComplexType.DeclaredStructuralProperties())
            {
                var taupoProperty = this.ConvertToTaupoProperty(edmProperty);
                taupoComplexType.Add(taupoProperty);
            }

            this.ConvertAnnotationsIntoTaupo(edmComplexType, taupoComplexType);
            return taupoComplexType;
        }

        private EntityType ConvertToTaupoEntityType(IEdmEntityType edmEntityType)
        {
            var taupoEntityType = new EntityType(edmEntityType.Namespace, edmEntityType.Name)
            {
                IsAbstract = edmEntityType.IsAbstract,
                IsOpen = edmEntityType.IsOpen,
            };

            if (edmEntityType.BaseType != null)
            {
                taupoEntityType.BaseType = new EntityTypeReference(edmEntityType.BaseEntityType().Namespace, edmEntityType.BaseEntityType().Name);
            }

            foreach (var edmProperty in edmEntityType.DeclaredStructuralProperties())
            {
                var taupoProperty = this.ConvertToTaupoProperty(edmProperty);
                taupoProperty.IsPrimaryKey = edmEntityType.Key().Contains(edmProperty);
                taupoEntityType.Add(taupoProperty);
            }

            this.ConvertAnnotationsIntoTaupo(edmEntityType, taupoEntityType);
            return taupoEntityType;
        }

        private MemberProperty ConvertToTaupoProperty(IEdmStructuralProperty edmProperty)
        {
            var taupoProperty = new MemberProperty(edmProperty.Name);
            taupoProperty.PropertyType = this.ConvertToTaupoDataType(edmProperty.Type);

            if (!string.IsNullOrEmpty(edmProperty.DefaultValueString))
            {
                taupoProperty.DefaultValue = edmProperty.DefaultValueString;
            }

            this.ConvertAnnotationsIntoTaupo(edmProperty, taupoProperty);
            return taupoProperty;
        }

        private DataType ConvertToTaupoDataType(IEdmTypeReference edmTypeReference)
        {
            EdmTypeKind kind = edmTypeReference.TypeKind();

            if (kind == EdmTypeKind.Collection)
            {
                var elementEdmTypeReference = edmTypeReference.AsCollection().ElementType();
                return DataTypes.CollectionType
                                 .WithElementDataType(this.ConvertToTaupoDataType(elementEdmTypeReference))
                                 .Nullable(edmTypeReference.IsNullable);
            }
            else if (kind == EdmTypeKind.Complex)
            {
                var complexEdmTypeDefinition = edmTypeReference.AsComplex().ComplexDefinition();
                return DataTypes.ComplexType
                                .WithName(complexEdmTypeDefinition.Namespace, complexEdmTypeDefinition.Name)
                                .Nullable(edmTypeReference.IsNullable);
            }
            else if (kind == EdmTypeKind.Entity)
            {
                var entityEdmTypeDefinition = edmTypeReference.AsEntity().EntityDefinition();
                return DataTypes.EntityType
                                .WithName(entityEdmTypeDefinition.Namespace, entityEdmTypeDefinition.Name)
                                .Nullable(edmTypeReference.IsNullable);
            }
            else if (kind == EdmTypeKind.EntityReference)
            {
                var entityEdmTypeDefinition = edmTypeReference.AsEntityReference().EntityType();
                return DataTypes.ReferenceType
                                .WithEntityType(new EntityTypeReference(entityEdmTypeDefinition.Namespace, entityEdmTypeDefinition.Name))
                                .Nullable(edmTypeReference.IsNullable);
            }
            else if (kind == EdmTypeKind.Primitive)
            {
                return EdmToTaupoPrimitiveDataTypeConverter.ConvertToTaupoPrimitiveDataType(edmTypeReference.AsPrimitive());
            }
            else if (kind == EdmTypeKind.Enum)
            {
                var enumTypeDefinition = edmTypeReference.AsEnum().EnumDefinition();
                return DataTypes.EnumType.WithName(enumTypeDefinition.Namespace, enumTypeDefinition.Name);
            }

            throw new TaupoInvalidOperationException("unexpected Edm Type Kind: " + kind);
        }

        private EntityContainer ConvertToTaupoEntityContainer(IEdmEntityContainer edmEntityContainer)
        {
            var taupoEntityContainer = new EntityContainer(edmEntityContainer.Name);

            foreach (var edmEntitySet in edmEntityContainer.Elements.OfType<IEdmEntitySet>())
            {
                EntitySet taupoEntitySet = this.ConvertToTaupoEntitySet(edmEntitySet);
                taupoEntityContainer.Add(taupoEntitySet);

                foreach (var edmNavigationProperty in edmEntitySet.NavigationPropertyBindings.Select(t => t.NavigationProperty))
                {
                    if (!this.associationRegistry.IsAssociationSetRegistered(edmEntitySet, edmNavigationProperty))
                    {
                        this.associationRegistry.RegisterAssociationSet(edmEntitySet, edmNavigationProperty);
                    }
                }
            }

            foreach (var edmFunctionImport in edmEntityContainer.Elements.OfType<IEdmOperationImport>())
            {
                FunctionImport taupoFunctionImport = this.ConvertToTaupoFunctionImport(edmFunctionImport);
                taupoEntityContainer.Add(taupoFunctionImport);
            }

            this.ConvertAnnotationsIntoTaupo(edmEntityContainer, taupoEntityContainer);
            return taupoEntityContainer;
        }

        private EntitySet ConvertToTaupoEntitySet(IEdmEntitySet edmEntitySet)
        {
            var taupoEntitySet = new EntitySet(edmEntitySet.Name);
            taupoEntitySet.EntityType = new EntityTypeReference(edmEntitySet.EntityType().Namespace, edmEntitySet.EntityType().Name);

            this.ConvertAnnotationsIntoTaupo(edmEntitySet, taupoEntitySet);
            return taupoEntitySet;
        }

        private FunctionImport ConvertToTaupoFunctionImport(IEdmOperationImport edmFunctionImport)
        {
            var taupoFunctionImport = new FunctionImport(edmFunctionImport.Name);
            var functionImportReturnType = new FunctionImportReturnType();
            var addReturnType = false;
            if (edmFunctionImport.EntitySet != null)
            {
                IEdmEntitySetBase entitySet;
                if (edmFunctionImport.TryGetStaticEntitySet(this.edmModel, out entitySet))
                {
                    functionImportReturnType.EntitySet = new EntitySetReference(entitySet.Name);
                    addReturnType = true;
                }
                else
                {
                    throw new NotSupportedException("Function import with entity set path is not supported.");
                }
            }

            if (edmFunctionImport.Operation.ReturnType != null)
            {
                functionImportReturnType.DataType = this.ConvertToTaupoDataType(edmFunctionImport.Operation.ReturnType);
                addReturnType = true;
            }

            if (addReturnType)
            {
                taupoFunctionImport.Add(functionImportReturnType);
            }

            foreach (var edmFunctionParameter in edmFunctionImport.Operation.Parameters)
            {
                FunctionParameter taupoFunctionParameter = this.ConvertToTaupoFunctionParameter(edmFunctionParameter);
                taupoFunctionImport.Parameters.Add(taupoFunctionParameter);
            }

            this.ConvertAnnotationsIntoTaupo(edmFunctionImport, taupoFunctionImport);
            return taupoFunctionImport;
        }

        private FunctionParameter ConvertToTaupoFunctionParameter(IEdmOperationParameter edmOperationParameter)
        {
            var taupoFunctionParameter = new FunctionParameter()
            {
                Name = edmOperationParameter.Name,
                DataType = this.ConvertToTaupoDataType(edmOperationParameter.Type),
                Mode = FunctionParameterMode.In
            };

            this.ConvertAnnotationsIntoTaupo(edmOperationParameter, taupoFunctionParameter);
            return taupoFunctionParameter;
        }

        private Function ConvertToTaupoFunction(IEdmOperation edmOperation)
        {
            var taupoFunction = new Function(edmOperation.Namespace, edmOperation.Name);

            if (edmOperation.ReturnType != null)
            {
                taupoFunction.ReturnType = this.ConvertToTaupoDataType(edmOperation.ReturnType);
            }

            foreach (var edmFunctionParameter in edmOperation.Parameters)
            {
                FunctionParameter taupoFunctionParameter = this.ConvertToTaupoFunctionParameter(edmFunctionParameter);
                taupoFunction.Parameters.Add(taupoFunctionParameter);
            }

            this.ConvertAnnotationsIntoTaupo(edmOperation, taupoFunction);
            return taupoFunction;
        }

        private EnumType ConvertToTaupoEnumType(IEdmEnumType edmEnum)
        {
            var taupoEnumType = new EnumType(edmEnum.Namespace, edmEnum.Name);
            if (edmEnum.IsFlags)
            {
                taupoEnumType.IsFlags = true;
            }

            if (edmEnum.UnderlyingType != null)
            {
                taupoEnumType.UnderlyingType = this.ConvertToClrType(edmEnum.UnderlyingType);
            }

            foreach (var edmEnumMember in edmEnum.Members)
            {
                var taupoEnumMember = new EnumMember(edmEnumMember.Name);

                if (edmEnumMember.Value != null)
                {
                    taupoEnumMember.Value = edmEnumMember.Value.Value;
                }

                taupoEnumType.Add(taupoEnumMember);
            }

            return taupoEnumType;
        }

        private Type ConvertToClrType(IEdmPrimitiveType edmPrimitiveType)
        {
            switch (edmPrimitiveType.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Int32:
                    // in the default case, no need to specify
                    return null;
                case EdmPrimitiveTypeKind.SByte:
                    return typeof(sbyte);
                case EdmPrimitiveTypeKind.Byte:
                    return typeof(byte);
                case EdmPrimitiveTypeKind.Int16:
                    return typeof(short);
                case EdmPrimitiveTypeKind.Int64:
                    return typeof(long);
                default:
                    throw new TaupoInvalidOperationException("Unsupported primitive type for Enum underlying type: " + edmPrimitiveType);
            }
        }

        private class AssociationRegistry
        {
            private List<IEdmNavigationProperty> registeredAssociations = new List<IEdmNavigationProperty>();
            private List<KeyValuePair<IEdmEntitySet, IEdmNavigationProperty>> registeredAssociationSets = new List<KeyValuePair<IEdmEntitySet, IEdmNavigationProperty>>();

            public void RegisterAssociation(IEdmNavigationProperty navigation)
            {
                this.registeredAssociations.Add(navigation);
            }

            public void RegisterAssociationSet(IEdmEntitySet entitySet, IEdmNavigationProperty navigation)
            {
                this.registeredAssociationSets.Add(new KeyValuePair<IEdmEntitySet, IEdmNavigationProperty>(entitySet, navigation));
            }

            public bool IsAssociationRegistered(IEdmNavigationProperty navigation)
            {
                return this.registeredAssociations.Contains(navigation) ||
                       this.registeredAssociations.Contains(navigation.Partner);
            }

            public bool IsAssociationSetRegistered(IEdmEntitySet entitySet, IEdmNavigationProperty navigation)
            {
                return this.registeredAssociationSets.Any(s => s.Key == entitySet && s.Value == navigation) ||
                       this.registeredAssociationSets.Any(s => s.Key == entitySet.FindNavigationTarget(navigation) && s.Value == navigation.Partner);
            }
        }
    }
}
