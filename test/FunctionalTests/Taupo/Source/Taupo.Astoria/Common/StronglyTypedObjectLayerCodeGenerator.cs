//---------------------------------------------------------------------
// <copyright file="StronglyTypedObjectLayerCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Object layer generator for use in strongly-typed scenarios
    /// </summary>
    public class StronglyTypedObjectLayerCodeGenerator : ObjectLayerCodeGeneratorBase
    {
        /// <summary>
        /// Initializes a new instance of the StronglyTypedObjectLayerCodeGenerator class
        /// </summary>
        /// <param name="backingTypeResolver">The backing type resolver to use</param>
        public StronglyTypedObjectLayerCodeGenerator(IClrTypeReferenceResolver backingTypeResolver)
            : base(backingTypeResolver)
        {
        }

        /// <summary>
        /// Gets the list of namespaces to import
        /// </summary>
        /// <returns>The namespaces needed for the objects</returns>
        protected override IList<string> GetNamespaceImports()
        {
            var list = base.GetNamespaceImports();
            list.Add("System"); // for Nullable<T>
            list.Add("System.Collections"); // for IEnumerable
            list.Add("System.Collections.Generic"); // for List<T>
            list.Add("Microsoft.OData.Client"); // for [DataServiceKey]
            list.Add("System.Linq"); // for IQueryable
            return list;
        }

        /// <summary>
        /// Fills in the given type declaration based on the given metadata
        /// </summary>
        /// <param name="complexType">The complex type's metadata</param>
        /// <param name="complexTypeClass">The type declaration</param>
        protected override void DeclareComplexType(ComplexType complexType, CodeTypeDeclaration complexTypeClass)
        {
            complexTypeClass.IsPartial = true;
            complexTypeClass.IsClass = true;

            // Add public constructor for this type
            var constructor = complexTypeClass.AddConstructor();

            foreach (MemberProperty memberProperty in complexType.Properties)
            {
                this.DeclareMemberProperty(memberProperty, complexTypeClass);
                this.DeclareOptionalPropertyInitializer(memberProperty, constructor);
            }
        }

        /// <summary>
        /// Fills in the given type declaration based on the given metadata
        /// </summary>
        /// <param name="entityType">The entity type's metadata</param>
        /// <param name="entityTypeClass">The type declaration</param>
        protected override void DeclareEntityType(EntityType entityType, CodeTypeDeclaration entityTypeClass)
        {
            entityTypeClass.IsPartial = true;
            entityTypeClass.IsClass = true;

            if (entityType.IsAbstract)
            {
                entityTypeClass.SetAbstract();
            }

            if (entityType.BaseType != null)
            {
                entityTypeClass.BaseTypes.Add(entityType.BaseType.FullName);
            }

            // Add public constructor for this type
            var constructor = entityTypeClass.AddConstructor();

            // Declare all members that are declared
            foreach (MemberProperty memberProperty in entityType.Properties)
            {
                this.DeclareMemberProperty(memberProperty, entityTypeClass);
                this.DeclareOptionalPropertyInitializer(memberProperty, constructor);
            }

            // Declare all members that are declared
            foreach (NavigationProperty navigationProperty in entityType.NavigationProperties)
            {
                this.DeclareNavigationProperty(navigationProperty, entityTypeClass);
                this.DeclareOptionalPropertyInitializer(navigationProperty, constructor);
            }

            this.GenerateAttributes(entityType, entityTypeClass);
        }

        /// <summary>
        /// Generates and adds attribute declarations to the entity type
        /// </summary>
        /// <param name="entityType">The entity type's metadata</param>
        /// <param name="entityTypeClass">The entity types declaration</param>
        protected virtual void GenerateAttributes(EntityType entityType, CodeTypeDeclaration entityTypeClass)
        {
            this.GenerateKeyAttribute(entityType, entityTypeClass);

            foreach (MemberProperty streamProperty in entityType.Properties.Where(p => p.IsStream()))
            {
                this.GenerateNamedStreamAttribute(entityTypeClass, streamProperty);
            }

            if (entityType.HasStream())
            {
                this.GenerateHasStreamEntityTypeCodeElements(entityType, entityTypeClass);
            }
        }

        /// <summary>
        /// Generates and adds stream related attributes and elements to the entity type
        /// </summary>
        /// <param name="entityType">The entity type's metadata</param>
        /// <param name="entityTypeClass">The entity types declaration</param>
        protected virtual void GenerateHasStreamEntityTypeCodeElements(EntityType entityType, CodeTypeDeclaration entityTypeClass)
        {
            ExceptionUtilities.Assert(entityType.HasStream(), "This method should not be called for entity types without stream.");
            entityTypeClass.AddCustomAttribute(Code.TypeRef("HasStream"));
        }

        /// <summary>
        /// Generates the [DataServiceKey] attribute on a given client type
        /// </summary>
        /// <param name="entityType">The complex type's metadata</param>
        /// <param name="entityTypeClass">The complex types declaration</param>
        protected virtual void GenerateKeyAttribute(EntityType entityType, CodeTypeDeclaration entityTypeClass)
        {
            if (entityType.BaseType == null)
            {
                var keyAttribute = entityTypeClass.AddCustomAttribute(Code.TypeRef("Key"));
                keyAttribute.Arguments.AddRange(entityType.AllKeyProperties.Select(p => new CodeAttributeArgument(Code.Primitive(p.Name))).ToArray());
            }
        }

        /// <summary>
        /// Adds a navigation property to the given type declaration based on the given metadata
        /// </summary>
        /// <param name="navigationProperty">The property's metadata</param>
        /// <param name="parentClass">The type declaration</param>
        protected override void DeclareNavigationProperty(NavigationProperty navigationProperty, CodeTypeDeclaration parentClass)
        {
            parentClass.AddAutoImplementedProperty(this.GetPropertyType(navigationProperty, CodeGenerationTypeUsage.Declaration), navigationProperty.Name);
        }

        /// <summary>
        /// Adds a property to the given type declaration based on the given metadata
        /// </summary>
        /// <param name="memberProperty">The property's metadata</param>
        /// <param name="parentClass">The type declaration</param>
        protected override void DeclareMemberProperty(MemberProperty memberProperty, CodeTypeDeclaration parentClass)
        {
            if (memberProperty.IsStream())
            {
                this.DeclareNamedStreamProperty(memberProperty, parentClass);
            }
            else
            {
                parentClass.AddAutoImplementedProperty(this.GetPropertyType(memberProperty, CodeGenerationTypeUsage.Declaration), memberProperty.Name);
            }
        }

        /// <summary>
        /// Adds a named stream property to the given type declaration based on the given metadata
        /// </summary>
        /// <param name="namedStreamProperty">The named stream property's metadata</param>
        /// <param name="parentClass">The type declaration</param>
        protected virtual void DeclareNamedStreamProperty(MemberProperty namedStreamProperty, CodeTypeDeclaration parentClass)
        {
        }

        /// <summary>
        /// Adds initialization for a property if any is needed
        /// </summary>
        /// <param name="memberProperty">The property to initialize</param>
        /// <param name="parentClassConstructor">The constructor to add to</param>
        protected virtual void DeclareOptionalPropertyInitializer(MemberProperty memberProperty, CodeConstructor parentClassConstructor)
        {
            // right now, we only need to initialize bag properties
            if (memberProperty.PropertyType is CollectionDataType)
            {
                var type = this.GetPropertyType(memberProperty, CodeGenerationTypeUsage.Instantiation);
                parentClassConstructor.Statements.Add(Code.This().Property(memberProperty.Name).Assign(Code.New(type)));
            }
        }

        /// <summary>
        /// Adds initialization for a property if any is needed
        /// </summary>
        /// <param name="navigationProperty">The property to initialize</param>
        /// <param name="parentClassConstructor">The constructor to add to</param>
        protected virtual void DeclareOptionalPropertyInitializer(NavigationProperty navigationProperty, CodeConstructor parentClassConstructor)
        {
            // right now, we only need to initialize collection navigations
            if (navigationProperty.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
            {
                var type = this.GetPropertyType(navigationProperty, CodeGenerationTypeUsage.Instantiation);
                parentClassConstructor.Statements.Add(Code.This().Property(navigationProperty.Name).Assign(Code.New(type)));
            }
        }

        private void GenerateNamedStreamAttribute(CodeTypeDeclaration entityTypeClass, MemberProperty streamProperty)
        {
            var epmAttribute = entityTypeClass.AddCustomAttribute(Code.TypeRef("NamedStream"));
            epmAttribute.Arguments.Add(new CodeAttributeArgument(Code.Primitive(streamProperty.Name)));
        }
    }
}