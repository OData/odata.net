//---------------------------------------------------------------------
// <copyright file="ObjectLayerCodeGeneratorBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Base implementation of the object layer generator contract
    /// </summary>
    public abstract class ObjectLayerCodeGeneratorBase : IObjectLayerCodeGenerator
    {
        /// <summary>
        /// Initializes a new instance of the ObjectLayerCodeGeneratorBase class
        /// </summary>
        /// <param name="backingTypeResolver">The type resolver</param>
        protected ObjectLayerCodeGeneratorBase(IClrTypeReferenceResolver backingTypeResolver)
        {
            ExceptionUtilities.CheckArgumentNotNull(backingTypeResolver, "backingTypeResolver");
            this.BackingTypeResolver = backingTypeResolver;
        }

        /// <summary>
        /// Gets the backing type resolver
        /// </summary>
        protected IClrTypeReferenceResolver BackingTypeResolver { get; private set; }

        /// <summary>
        /// Generates the object layer for the given model
        /// </summary>
        /// <param name="compileUnit">The compile unit to add code to</param>
        /// <param name="model">The model to base the object layer on</param>
        public void GenerateObjectLayer(CodeCompileUnit compileUnit, EntityModelSchema model)
        {
            var namespaces = model.EntityTypes.Select(et => et.NamespaceName).Union(model.ComplexTypes.Select(ct => ct.NamespaceName)).Union(model.Functions.Select(f => f.NamespaceName)).Distinct().ToList();

            foreach (string namespaceName in namespaces)
            {
                CodeNamespace codeNamespace = Code.AddNamespace(compileUnit, namespaceName);

                foreach (string ns in this.GetNamespaceImports())
                {
                    codeNamespace.ImportNamespace(ns);
                }

                foreach (ComplexType type in model.ComplexTypes.Where(ct => ct.NamespaceName == namespaceName))
                {
                    if (this.ShouldGenerateTypeDefinition(type))
                    {
                        this.DeclareComplexType(type, codeNamespace.DeclareType(type.Name));
                    }
                }

                foreach (EntityType type in model.EntityTypes.Where(ct => ct.NamespaceName == namespaceName))
                {
                    if (this.ShouldGenerateTypeDefinition(type))
                    {
                        this.DeclareEntityType(type, codeNamespace.DeclareType(type.Name));
                    }
                }

                this.AddFunctionsInNamespaceIfRequired(namespaceName, codeNamespace, model.Functions);
            }
        }

        /// <summary>
        /// Adds functions to the code namespace
        /// </summary>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="codeNamespace">Code Names space</param>
        /// <param name="functions">Functions from the model</param>
        protected virtual void AddFunctionsInNamespaceIfRequired(string namespaceName, CodeNamespace codeNamespace, IEnumerable<Function> functions)
        {
        }

        /// <summary>
        /// Gets the list of namespaces to import
        /// </summary>
        /// <returns>The namespaces needed for the objects</returns>
        protected virtual IList<string> GetNamespaceImports()
        {
            return new List<string>();
        }

        /// <summary>
        /// Fills in the given type declaration based on the given metadata
        /// </summary>
        /// <param name="complexType">The complex type's metadata</param>
        /// <param name="complexTypeClass">The type declaration</param>
        protected abstract void DeclareComplexType(ComplexType complexType, CodeTypeDeclaration complexTypeClass);

        /// <summary>
        /// Fills in the given type declaration based on the given metadata
        /// </summary>
        /// <param name="entityType">The entity type's metadata</param>
        /// <param name="entityTypeClass">The type declaration</param>
        protected abstract void DeclareEntityType(EntityType entityType, CodeTypeDeclaration entityTypeClass);

        /// <summary>
        /// Adds a property to the given type declaration based on the given metadata
        /// </summary>
        /// <param name="memberProperty">The property's metadata</param>
        /// <param name="parentClass">The type declaration</param>
        protected abstract void DeclareMemberProperty(MemberProperty memberProperty, CodeTypeDeclaration parentClass);

        /// <summary>
        /// Adds a navigation property to the given type declaration based on the given metadata
        /// </summary>
        /// <param name="navigationProperty">The property's metadata</param>
        /// <param name="parentClass">The type declaration</param>
        protected abstract void DeclareNavigationProperty(NavigationProperty navigationProperty, CodeTypeDeclaration parentClass);

        /// <summary>
        /// Returns the type to use when declaring or instantiating the property
        /// </summary>
        /// <param name="property">The property being declared/instantiated</param>
        /// <param name="usage">Whether the type is for declaration or instantiation</param>
        /// <returns>The type of the property</returns>
        protected CodeTypeReference GetPropertyType(MemberProperty property, CodeGenerationTypeUsage usage)
        {
            var collectionDataType = property.PropertyType as CollectionDataType;
            if (collectionDataType != null)
            {
                return this.GetCollectionType(usage, property.Annotations, collectionDataType.ElementDataType);
            }

            return this.BackingTypeResolver.ResolveClrTypeReference(property.PropertyType);
        }

        /// <summary>
        /// Returns the type to use when declaring or instantiating the property
        /// </summary>
        /// <param name="property">The property being declared/instantiated</param>
        /// <param name="usage">The usage for the type (either for instantiation or declaration)</param>
        /// <returns>The type of the property</returns>
        protected CodeTypeReference GetPropertyType(NavigationProperty property, CodeGenerationTypeUsage usage)
        {
            var type = DataTypes.EntityType.WithDefinition(property.ToAssociationEnd.EntityType);
            if (property.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
            {
                return this.GetCollectionType(usage, property.Annotations, type);
            }

            return this.BackingTypeResolver.ResolveClrTypeReference(type);
        }

        /// <summary>
        /// Gets a type reference to use when declaring or instantiation a collection property
        /// </summary>
        /// <param name="usage">Whether the type is for declaration or instantiation</param>
        /// <param name="propertyAnnotations">The annotations of the property</param>
        /// <param name="elementDataType">The element type of the collection</param>
        /// <returns>The type of the property</returns>
        protected virtual CodeTypeReference GetCollectionType(CodeGenerationTypeUsage usage, IEnumerable<Annotation> propertyAnnotations, DataType elementDataType)
        {
            // look up the type annotation in the given annotations for this usage
            var annotation = propertyAnnotations.OfType<CollectionTypeAnnotation>().SingleOrDefault(t => t.Usage == usage);
            ExceptionUtilities.CheckObjectNotNull(annotation, "Property did not have a collection type defined with usage: {0}", usage);

            // if it is generic, then use the backing type of the given element type as the generic argument
            if (annotation.IsGeneric)
            {
                return Code.GenericType(annotation.FullTypeName, this.BackingTypeResolver.ResolveClrTypeReference(elementDataType));
            }
            else
            {
                return Code.TypeRef(annotation.FullTypeName);
            }
        }

        /// <summary>
        /// Returns whether or not to generate a clr type for the given model type
        /// </summary>
        /// <param name="type">The model type</param>
        /// <returns>Whether or not to generate a clr type</returns>
        protected virtual bool ShouldGenerateTypeDefinition(StructuralType type)
        {
            return true;
        }
    }
}