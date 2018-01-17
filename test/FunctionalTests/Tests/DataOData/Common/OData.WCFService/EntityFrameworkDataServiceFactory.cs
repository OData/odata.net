//---------------------------------------------------------------------
// <copyright file="EntityFrameworkDataServiceFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using EdmProperty = System.Data.Metadata.Edm.EdmProperty;

    /// <summary>
    /// This factory class creates an IODataMetadataProvider given an Object context and its metadata workspace
    /// </summary>
    public static class EntityFrameworkDataServiceFactory
    {
        /// <summary>
        /// Creates an <see cref="IEdmModel"/> based on a combination of the ObjectContext's properties and it's metadata workspace
        /// </summary>
        /// <param name="context">The object context instance</param>
        /// <returns>An <see cref="IEdmModel"/> based on a combination of the ObjectContext's properties and it's metadata workspace</returns>
        public static IEdmModel CreateModel(ObjectContext context)
        {
            Assembly objectLayerAssembly = context.GetType().Assembly;
            ConstructableMetadata metadata = new ConstructableMetadata(context.DefaultContainerName, ("DBNorthwindModel"));
            foreach (EntityType entityType in context.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace))
            {
                Type clrType = objectLayerAssembly.GetType("Microsoft.Test.Taupo.OData.WCFService.Model." + entityType.Name, false, true);
                IEdmEntityType edmEntityType = metadata.AddEntityType(entityType.Name, clrType, null, false, "DBNorthwindModel");
                AddProperties(metadata, entityType.Properties, edmEntityType);
            }

            foreach (var entitySet in context.GetType().GetProperties().Where(property => property.PropertyType.GetInterface("IQueryable") != null))
            {
                Type entityType = entitySet.PropertyType.GetGenericArguments().First();
                IEdmEntityType edmEntityType = metadata.FindType("DBNorthwindModel." + entityType.Name) as IEdmEntityType;
                metadata.AddEntitySet(entitySet.Name, edmEntityType);
            }

            return metadata;
        }

        /// <summary>
        /// Adds Associations to the EDM model based on an Entity Data Model Schema
        /// </summary>
        /// <param name="model">EDM model to append to</param>
        /// <param name="model">Entity Data Model Schema to get associations from</param>
        private static void AddNavigationProperties(ConstructableMetadata model, IEdmEntityType entityType, IEnumerable<NavigationProperty> properties)
        {
            foreach (NavigationProperty property in properties)
            {
                if (property.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                {
                    model.AddResourceSetReferenceProperty(
                        entityType,
                        property.Name,
                        model.FindEntitySet(property.ToEndMember.GetEntityType().Name),
                        model.FindType(property.ToEndMember.GetEntityType().FullName) as IEdmEntityType
                    );
                }
                else
                {
                    model.AddResourceReferenceProperty(
                        entityType,
                        property.Name,
                        model.FindEntitySet(property.ToEndMember.GetEntityType().Name),
                        model.FindType(property.ToEndMember.GetEntityType().FullName) as IEdmEntityType
                    );
                }
            }
        }

        /// <summary>
        /// Adds a Primitive Property to <paramref name="metadata"/>
        /// </summary>
        /// <param name="metadata">Metadata document to be added to</param>
        /// <param name="type">The type of the primitive property</param>
        /// <param name="property">Property to add</param>
        /// <param name="structuredType">Structured type to add property to</param>
        private static void AddPrimitiveProperty(ConstructableMetadata metadata, Type type, EdmProperty property, IEdmStructuredType structuredType)
        {
            if (property.DeclaringType is EntityType && ((EntityType)property.DeclaringType).KeyMembers.Contains(property.Name))
            {
                metadata.AddKeyProperty(structuredType, property.Name, type);
            }
            else if (type == typeof(Stream))
            {
                metadata.AddNamedStreamProperty(structuredType, property.Name);
            }
            else
            {
                metadata.AddPrimitiveProperty(structuredType, property.Name, type);
            }
        }

        /// <summary>
        /// Adds a <paramref name="property"/> to <paramref name="metadata"/>
        /// under type <paramref name="structuredType"/>
        /// </summary>
        /// <param name="metadata">Metadata document to be added to</param>
        /// <param name="property">Property to add</param>
        /// <param name="structuredType">Structured type to add property to</param>
        private static void AddProperty(ConstructableMetadata metadata, EdmProperty property, IEdmStructuredType structuredType)
        {
            if (property.BuiltInTypeKind == BuiltInTypeKind.EdmProperty)
            {
                AddPrimitiveProperty(metadata, DataTypeToSystemType((PrimitiveType)property.TypeUsage.EdmType, IsNullable(property.TypeUsage)), property, structuredType);
            }
            else if (property.BuiltInTypeKind == BuiltInTypeKind.ComplexType)
            {
                metadata.AddComplexProperty(structuredType, property.Name, metadata.FindType(property.TypeUsage.EdmType.FullName) as IEdmComplexType);
            }
        }

        /// <summary>
        /// Adds the <paramref name="properties"/> to the <paramref name="metadata"/>
        /// Recursively calls self to add complex types as needed
        /// </summary>
        /// <param name="metadata">The metadata document to add properties to</param>
        /// <param name="properties">The properties to add to the metadata document</param>
        /// <param name="structuredType">The type that these elements belong to</param>
        private static void AddProperties(ConstructableMetadata metadata, IEnumerable<EdmProperty> properties, IEdmStructuredType structuredType)
        {
            foreach (EdmProperty property in properties)
            {
                AddProperty(metadata, property, structuredType);
            }
        }

        /// <summary>
        /// Converts a variable of type 'DataType' to type 'System.Type'
        /// </summary>
        /// <param name="type">type to convert</param>
        /// <returns>System.Type representation of type</returns>
        private static Type DataTypeToSystemType(PrimitiveType type, bool makeNullable)
        {
            if (makeNullable && type.ClrEquivalentType != typeof(string) && type.ClrEquivalentType != typeof(byte[]))
            {
                return typeof(Nullable<>).MakeGenericType(type.ClrEquivalentType);
            }

            return type.ClrEquivalentType;
        }

        private static bool IsNullable(TypeUsage typeUsage)
        {
            bool isTypeNullable = typeUsage.Facets.Any(facet => facet.Name == "Nullable" && bool.Parse(facet.Value.ToString()));
            return isTypeNullable;
        }
    }
}