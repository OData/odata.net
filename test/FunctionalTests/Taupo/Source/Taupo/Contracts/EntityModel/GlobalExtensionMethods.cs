//---------------------------------------------------------------------
// <copyright file="GlobalExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Global extension methods to simplify usage of EntityModel classes.
    /// </summary>
    /// <remarks>
    /// The class is declared within contracts to reduce the number of using statements that
    /// are necessary in user code. Layering test verifies that those methods can be called only 
    /// from the user code but not from Taupo code.
    /// </remarks>
    public static class GlobalExtensionMethods
    {
        /// <summary>
        /// Resolves named references within the schema.
        /// </summary>
        /// <param name="schema">Schema to resolve.</param>
        /// <returns>This instance (can be used to chain calls together)</returns>
        public static EntityModelSchema Resolve(this EntityModelSchema schema)
        {
            var fixup = new ResolveReferencesFixup();
            fixup.Fixup(schema);

            var enumDataTypeResolver = new EnumDataTypeResolver(schema);
            enumDataTypeResolver.ResolveEnumDataTypes();

            return schema;
        }

        /// <summary>
        /// Applies default namespace to all items in the model that don't have one.
        /// </summary>
        /// <param name="schema">Schema to fixup.</param>
        /// <param name="defaultNamespaceName">Default namespace to apply.</param>
        /// <returns>This instance (can be used to chain calls together)</returns>
        public static EntityModelSchema ApplyDefaultNamespace(this EntityModelSchema schema, string defaultNamespaceName)
        {
            var fixup = new ApplyDefaultNamespaceFixup(defaultNamespaceName);
            fixup.Fixup(schema);
            return schema;
        }

        /// <summary>
        /// Overrides the namespace of all items in the model
        /// </summary>
        /// <param name="schema">Schema to fixup.</param>
        /// <param name="newNamespaceName">New namespace to apply</param>
        /// <returns>This instance (can be used to chain calls together)</returns>
        public static EntityModelSchema OverrideNamespace(this EntityModelSchema schema, string newNamespaceName)
        {
            var fixup = new OverrideNamespaceFixup(newNamespaceName);
            fixup.Fixup(schema);
            return schema;
        }

        /// <summary>
        /// Add the default container to the model
        ///   By default is having one set for each base EntityType and AssociationType, you can opt out by adding FixupNoSetAnnotation
        /// </summary>
        /// <param name="schema">Schema to add Container</param>
        /// <param name="containerName">name of Container</param>
        /// <returns>This instance (can be used to chain calls together)</returns>
        public static EntityModelSchema AddDefaultContainer(this EntityModelSchema schema, string containerName)
        {
            var fixup = new AddDefaultContainerFixup(containerName);
            fixup.Fixup(schema);
            return schema;
        }

        /// <summary>
        /// Add default Navigation Properties to the model
        ///   By default bi-directional, you can opt out by adding FixupNoNavigationAnnotation
        /// </summary>
        /// <param name="schema">Schema to add default Navigation Properties</param>
        /// <returns>This instance (can be used to chain calls together)</returns>
        public static EntityModelSchema AddDefaultNavigationProperties(this EntityModelSchema schema)
        {
            var fixup = new AddNavigationPropertyFixup(new ConsecutiveIdentifierGenerator());
            fixup.Fixup(schema);
            return schema;
        }

        /// <summary>
        /// Gets the navigation property that starts at this <see cref="AssociationEnd"/>
        /// and navigates to the other <see cref="AssociationEnd"/>.
        /// </summary>
        /// <param name="end">The end to find the navigation property for</param>
        /// <returns>The navigation property, Null if no navigation is defined</returns>
        public static NavigationProperty FromNavigationProperty(this AssociationEnd end)
        {
            ExceptionUtilities.CheckArgumentNotNull(end, "end");
            return end.EntityType.NavigationProperties.Where(n => n.FromAssociationEnd == end).SingleOrDefault();
        }

        /// <summary>
        /// Returns an abstraction that facilitates access to association and association
        /// end data, including navigation properties, relationship metadata, and dependent/principal
        /// information, for a particular <see cref="AssociationSet"/>
        /// </summary>
        /// <param name="associationSet">The <see cref="AssociationSet"/> from which to build a <see cref="RelationshipType"/>.</param>
        /// <returns>The <see cref="RelationshipType"/> object representing a particular <see cref="AssociationSet"/>.</returns>
        public static RelationshipType RelationshipType(this AssociationSet associationSet)
        {
            ExceptionUtilities.CheckArgumentNotNull(associationSet, "associationSet");

            var annotation = associationSet.Annotations.OfType<RelationshipTypeAnnotation>().SingleOrDefault();
            if (annotation == null)
            {
                annotation = new RelationshipTypeAnnotation(new RelationshipType(associationSet));
                associationSet.Annotations.Add(annotation);
            }

            return annotation.RelationshipType;
        }

        /// <summary>
        /// Assesses all the associations in the specified <see cref="EntityContainer"/>
        /// and returns an abstraction that facilitates access to association and association
        /// end data, including navigation properties, relationship metadata, and dependent/principal
        /// information.
        /// </summary>
        /// <param name="entityContainer">The <see cref="EntityContainer"/> from which to determine relationships.</param>
        /// <returns>A collection of <see cref="RelationshipType"/>s that can be further queried
        /// to meet the consumer's needs.</returns>
        public static IEnumerable<RelationshipType> RelationshipTypes(this EntityContainer entityContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityContainer, "entityContainer");

            return entityContainer.AssociationSets.Select(a => a.RelationshipType());
        }

        /// <summary>
        /// Adds data generation hints for the given function parameter.
        /// </summary>
        /// <param name="functionParameter">Function parameter to add data generation hints to.</param>
        /// <param name="hints">Data generation hints.</param>
        /// <returns>This instance (can be used to chain calls together).</returns>
        public static FunctionParameter WithDataGenerationHints(this FunctionParameter functionParameter, params DataGenerationHint[] hints)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionParameter, "functionParameter");
            return functionParameter.WithDataGenerationHintsInternal(hints);
        }

        /// <summary>
        /// Adds data generation hints for the given property.
        /// </summary>
        /// <param name="property">Property to add data generation hints to.</param>
        /// <param name="hints">Data generation hints.</param>
        /// <returns>This instance (can be used to chain calls together).</returns>
        public static MemberProperty WithDataGenerationHints(this MemberProperty property, params DataGenerationHint[] hints)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            return property.WithDataGenerationHintsInternal(hints);
        }

        /// <summary>
        /// Sets the data generator for the given property and returns the property back.
        /// </summary>
        /// <param name="property">The property to set the data generator for.</param>
        /// <param name="dataGenerator">The data generator.</param>
        /// <returns>This instance (can be used to chain calls together).</returns>
        public static MemberProperty WithDataGenerator(this MemberProperty property, IDataGenerator dataGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            return property.WithDataGeneratorInternal(dataGenerator);
        }

        /// <summary>
        /// Sets the data generator for the given function parameter and returns the parameter back.
        /// </summary>
        /// <param name="functionParameter">The function parameter to set the data generator for.</param>
        /// <param name="dataGenerator">The data generator.</param>
        /// <returns>This instance (can be used to chain calls together).</returns>
        public static FunctionParameter WithDataGenerator(this FunctionParameter functionParameter, IDataGenerator dataGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionParameter, "functionParameter");

            return functionParameter.WithDataGeneratorInternal(dataGenerator);
        }

        /// <summary>
        /// Gets the named item by name from the items collection.
        /// First it tries to find item by full name, if not found then tries to find by name.
        /// </summary>
        /// <typeparam name="TNamedItem">The type of named item.</typeparam>
        /// <param name="items">The items collection where to search.</param>
        /// <param name="itemName">The item name.</param>
        /// <returns>The found item.</returns>
        public static TNamedItem GetNamedItemByName<TNamedItem>(this IEnumerable<TNamedItem> items, string itemName) where TNamedItem : INamedItem
        {
            var namedItem = items.FindNamedItemByName<TNamedItem>(itemName);

            ExceptionUtilities.CheckObjectNotNull(namedItem, "Cannot find {0} with name '{1}'.", typeof(TNamedItem).Name, itemName);

            return namedItem;
        }

        /// <summary>
        /// Finds the named item by name in the items collection.
        /// First it tries to find item by full name, if not found then tries to find by name.
        /// </summary>
        /// <typeparam name="TNamedItem">The type of named item.</typeparam>
        /// <param name="items">The items collection where to search.</param>
        /// <param name="itemName">The item name.</param>
        /// <returns>The found item or null if item is not found.</returns>
        public static TNamedItem FindNamedItemByName<TNamedItem>(this IEnumerable<TNamedItem> items, string itemName) where TNamedItem : INamedItem
        {
            ExceptionUtilities.CheckArgumentNotNull(items, "items");

            // Try to find by full name first
            var namedItem = items.SingleOrDefault(t => t.FullName == itemName);
            if (namedItem == null)
            {
                // Try to find by name
                var matchingItems = items.Where(t => t.Name == itemName).ToList();
                ExceptionUtilities.Assert(matchingItems.Count < 2, "Found more than one {0} with name '{1}'.", typeof(TNamedItem).Name, itemName);
                namedItem = matchingItems.SingleOrDefault();
            }

            return namedItem;
        }

        /// <summary>
        /// Finds all functions matching given namespace and name.
        /// </summary>
        /// <param name="schema">Schema to search.</param>
        /// <param name="functionNamespace">The function namespace.</param>
        /// <param name="functionName">The function name.</param>
        /// <returns>Functions that match given namespace and name.</returns>
        public static IEnumerable<Function> FindFunctions(this EntityModelSchema schema, string functionNamespace, string functionName)
        {
            ExceptionUtilities.CheckArgumentNotNull(schema, "schema");
            return schema.Functions.Where(f => f.NamespaceName == functionNamespace && f.Name == functionName);
        }

        /// <summary>
        /// Gets property paths for given entity type applying provided filter.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="filter">The filter to control which properties should be included.</param>
        /// <returns>Property paths.</returns>
        public static IEnumerable<string> GetPropertyPaths(this EntityType entityType, Func<string, MemberProperty, bool> filter)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            var propertyPaths = new List<string>();
            GetPropertyPaths(propertyPaths, string.Empty, entityType.AllProperties, filter);
            return propertyPaths;
        }

        /// <summary>
        /// Determines if the given property is a foreign key in the specified entity set.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <returns>True if a given property is foreign key, false otherwise.</returns>
        public static bool IsForeignKey(this MemberProperty property, EntitySet entitySet)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckObjectNotNull(entitySet.Container, "The entity set '{0}' is not associated with an entity container.", entitySet.Name);

            return entitySet.Container.AssociationSets.Any(a =>
                    a.AssociationType.ReferentialConstraint != null &&
                    a.AssociationType.ReferentialConstraint.DependentProperties.Contains(property) &&
                    entitySet == a.Ends.Single(e => e.AssociationEnd == a.AssociationType.ReferentialConstraint.DependentAssociationEnd).EntitySet);
        }

        /// <summary>
        /// Determines if the given property participating in any unique constraint for the specified entity.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>True if the given property participating in a unique constraint, false otherwise.</returns>
        public static bool IsUnique(this MemberProperty property, EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            return entityType.AllEdmUniqueConstraints.Any(c => c.Properties.Contains(property));
        }

        /// <summary>
        /// Determines if the given property is store-generated.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>True if the given property is store-generated, false otherwise.</returns>
        public static bool IsStoreGenerated(this MemberProperty property)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            return property.Annotations.OfType<StoreGeneratedPatternAnnotation>().Any(a => a.ServerGeneratedOnInsert || a.ServerGeneratedOnUpdate);
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="EntityModelSchema"/>.
        /// </summary>
        /// <param name="schema">The <see cref="EntityModelSchema"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="EntityModelSchema"/>.</returns>
        public static EntityModelSchema Clone(this EntityModelSchema schema)
        {
            ExceptionUtilities.CheckArgumentNotNull(schema, "schema");

            var model = new EntityModelSchema();
            CopyAnnotations(model, schema.Annotations);

            foreach (var association in schema.Associations)
            {
                model.Add(association.Clone());
            }

            foreach (var complexType in schema.ComplexTypes)
            {
                model.Add(complexType.Clone());
            }

            foreach (var entityContainer in schema.EntityContainers)
            {
                model.Add(entityContainer.Clone());
            }

            foreach (var entityType in schema.EntityTypes)
            {
                model.Add(entityType.Clone());
            }

            foreach (var enumType in schema.EnumTypes)
            {
                model.Add(enumType.Clone());
            }

            foreach (var function in schema.Functions)
            {
                model.Add(function.Clone());
            }

            model.Resolve();

            return model;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="EnumType"/>.
        /// </summary>
        /// <param name="enumType">The <see cref="EnumType"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="EnumType"/>.</returns>
        public static EnumType Clone(this EnumType enumType)
        {
            var clone = new EnumType(enumType.NamespaceName, enumType.Name)
            {
                IsFlags = enumType.IsFlags,
                UnderlyingType = enumType.UnderlyingType
            };

            foreach (var enumMember in enumType.Members)
            {
                clone.Add(enumMember.Clone());
            }

            CopyAnnotations(clone, enumType.Annotations);

            return clone;
        }

        private static void GetPropertyPaths(IList<string> list, string path, IEnumerable<MemberProperty> properties, Func<string, MemberProperty, bool> filter)
        {
            foreach (MemberProperty property in properties.Where(p => filter == null || filter(path + p.Name, p)))
            {
                ComplexDataType complexDataType = property.PropertyType as ComplexDataType;

                if (complexDataType == null)
                {
                    list.Add(path + property.Name);
                }
                else
                {
                    GetPropertyPaths(list, path + property.Name + ".", complexDataType.Definition.Properties, filter);
                }
            }
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="AssociationEnd"/>.
        /// </summary>
        /// <param name="associationEnd">The <see cref="AssociationEnd"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="AssociationEnd"/>.</returns>
        private static AssociationEnd Clone(this AssociationEnd associationEnd)
        {
            var clone = new AssociationEnd()
            {
                DeleteBehavior = associationEnd.DeleteBehavior,
                Multiplicity = associationEnd.Multiplicity,
                RoleName = associationEnd.RoleName,
                EntityType = associationEnd.EntityType.Name
            };

            CopyAnnotations(clone, associationEnd.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="AssociationSet"/>.
        /// </summary>
        /// <param name="associationSet">The <see cref="AssociationSet"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="AssociationSet"/>.</returns>
        private static AssociationSet Clone(this AssociationSet associationSet)
        {
            var clone = new AssociationSet(associationSet.Name, associationSet.AssociationType.Name);
            CopyAnnotations(clone, associationSet.Annotations);

            foreach (var associationSetEnd in associationSet.Ends)
            {
                clone.Add(associationSetEnd.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="AssociationSetEnd"/>.
        /// </summary>
        /// <param name="associationSetEnd">The <see cref="AssociationSetEnd"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="AssociationSetEnd"/>.</returns>
        private static AssociationSetEnd Clone(this AssociationSetEnd associationSetEnd)
        {
            var clone = new AssociationSetEnd(associationSetEnd.AssociationEnd.RoleName, associationSetEnd.EntitySet.NameOrNull());
            CopyAnnotations(clone, associationSetEnd.Annotations);

            return clone;
        }

        private static AssociationType Clone(this AssociationType associationType)
        {
            var clone = new AssociationType()
            {
                NamespaceName = associationType.NamespaceName,
                Name = associationType.Name
            };

            foreach (var associationEnd in associationType.Ends)
            {
                clone.Add(associationEnd.Clone());
            }

            clone.ReferentialConstraint = associationType.ReferentialConstraint.Clone();

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="ComplexType"/>.
        /// </summary>
        /// <param name="complexType">The <see cref="ComplexType"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="ComplexType"/>.</returns>
        private static ComplexType Clone(this ComplexType complexType)
        {
            var clone = new ComplexType()
            {
                BaseType = complexType.BaseType.NameOrNull(),
                NamespaceName = complexType.NamespaceName,
                Name = complexType.Name,
            };

            foreach (var property in complexType.Properties)
            {
                clone.Add(property.Clone());
            }

            CopyAnnotations(clone, complexType.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="EdmUniqueConstraint"/>.
        /// </summary>
        /// <param name="edmUniqueConstraint">The <see cref="EdmUniqueConstraint"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="EdmUniqueConstraint"/>.</returns>
        private static EdmUniqueConstraint Clone(this EdmUniqueConstraint edmUniqueConstraint)
        {
            var clone = new EdmUniqueConstraint(edmUniqueConstraint.Name)
                .WithProperties(edmUniqueConstraint.Properties.Select(p => p.Name).ToArray());

            CopyAnnotations(clone, edmUniqueConstraint.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="EntityContainer"/>.
        /// </summary>
        /// <param name="entityContainer">The <see cref="EntityContainer"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="EntityContainer"/>.</returns>
        private static EntityContainer Clone(this EntityContainer entityContainer)
        {
            var clone = new EntityContainer(entityContainer.Name);
            CopyAnnotations(clone, entityContainer.Annotations);

            foreach (var entitySet in entityContainer.EntitySets)
            {
                clone.Add(entitySet.Clone());
            }

            foreach (var associationSet in entityContainer.AssociationSets)
            {
                clone.Add(associationSet.Clone());
            }

            foreach (var functionImport in entityContainer.FunctionImports)
            {
                clone.Add(functionImport.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="EntityType"/>.
        /// </summary>
        /// <param name="entityType">The <see cref="EntityType"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="EntityType"/>.</returns>
        private static EntityType Clone(this EntityType entityType)
        {
            var clone = new EntityType(entityType.NamespaceName, entityType.Name)
            {
                BaseType = entityType.BaseType.NameOrNull(),
                IsAbstract = entityType.IsAbstract,
                IsOpen = entityType.IsOpen,
            };

            foreach (var property in entityType.Properties)
            {
                clone.Add(property.Clone());
            }

            foreach (var navigationProperty in entityType.NavigationProperties)
            {
                clone.Add(navigationProperty.Clone());
            }

            foreach (var uniqueConstraint in entityType.EdmUniqueConstraints)
            {
                clone.Add(uniqueConstraint.Clone());
            }

            CopyAnnotations(clone, entityType.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="EntitySet"/>.
        /// </summary>
        /// <param name="entitySet">The <see cref="EntitySet"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="EntitySet"/>.</returns>
        private static EntitySet Clone(this EntitySet entitySet)
        {
            var clone = new EntitySet(entitySet.Name, entitySet.EntityType.NameOrNull());
            CopyAnnotations(clone, entitySet.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="EnumMember"/>.
        /// </summary>
        /// <param name="enumMember">The <see cref="EnumMember"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="EnumMember"/>.</returns>
        private static EnumMember Clone(this EnumMember enumMember)
        {
            var clone = new EnumMember(enumMember.Name, enumMember.Value);

            CopyAnnotations(clone, enumMember.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="Function"/>.
        /// </summary>
        /// <param name="function">The <see cref="Function"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="Function"/>.</returns>
        private static Function Clone(this Function function)
        {
            var clone = new Function(function.NamespaceName, function.Name)
            {
                ReturnType = function.ReturnType.Clone()
            };

            CopyAnnotations(clone, function.Annotations);

            foreach (var parameter in function.Parameters)
            {
                clone.Add(parameter.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="FunctionImport"/>.
        /// </summary>
        /// <param name="functionImport">The <see cref="FunctionImport"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="FunctionImport"/>.</returns>
        private static FunctionImport Clone(this FunctionImport functionImport)
        {
            var clone = new FunctionImport(functionImport.Name)
            {
                IsComposable = functionImport.IsComposable,
                IsBindable = functionImport.IsBindable,
                IsSideEffecting = functionImport.IsSideEffecting,
            };

            CopyAnnotations(clone, functionImport.Annotations);

            foreach (var parameter in functionImport.Parameters)
            {
                clone.Add(parameter.Clone());
            }

            foreach (var returnType in functionImport.ReturnTypes)
            {
                clone.Add(new FunctionImportReturnType(returnType.DataType.Clone(), returnType.EntitySet.NameOrNull()));
            }

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="FunctionParameter"/>.
        /// </summary>
        /// <param name="functionParameter">The <see cref="FunctionParameter"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="FunctionParameter"/>.</returns>
        private static FunctionParameter Clone(this FunctionParameter functionParameter)
        {
            var clone = new FunctionParameter(functionParameter.Name, functionParameter.DataType.Clone(), functionParameter.Mode);
            CopyAnnotations(clone, functionParameter.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="MemberProperty"/>.
        /// </summary>
        /// <param name="memberProperty">The <see cref="MemberProperty"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="MemberProperty"/>.</returns>
        private static MemberProperty Clone(this MemberProperty memberProperty)
        {
            var clone = new MemberProperty
            {
                Name = memberProperty.Name,
                PropertyType = memberProperty.PropertyType.Clone(),
                IsPrimaryKey = memberProperty.IsPrimaryKey,
                DefaultValue = memberProperty.DefaultValue,
            };

            CopyAnnotations(clone, memberProperty.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="NavigationProperty"/>.
        /// </summary>
        /// <param name="navigationProperty">The <see cref="NavigationProperty"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="NavigationProperty"/>.</returns>
        private static NavigationProperty Clone(this NavigationProperty navigationProperty)
        {
            var clone = new NavigationProperty(
                navigationProperty.Name,
                navigationProperty.Association.Name,
                navigationProperty.FromAssociationEnd.RoleName,
                navigationProperty.ToAssociationEnd.RoleName);

            CopyAnnotations(clone, navigationProperty.Annotations);

            return clone;
        }

        /// <summary>
        /// Performs a deep copy of the specified <see cref="ReferentialConstraint"/>.
        /// </summary>
        /// <param name="referentialConstraint">The <see cref="ReferentialConstraint"/> to deep copy.</param>
        /// <returns>A deep copy of the <see cref="ReferentialConstraint"/>.</returns>
        private static ReferentialConstraint Clone(this ReferentialConstraint referentialConstraint)
        {
            if (referentialConstraint == null)
            {
                return null;
            }

            var clone = new ReferentialConstraint()
                .WithDependentProperties(
                    referentialConstraint.DependentAssociationEnd.RoleName,
                    referentialConstraint.DependentProperties.Select(m => new MemberPropertyReference(m.Name)).ToArray())
                .ReferencesPrincipalProperties(
                    referentialConstraint.PrincipalAssociationEnd.RoleName,
                    referentialConstraint.PrincipalProperties.Select(m => new MemberPropertyReference(m.Name)).ToArray());

            CopyAnnotations(clone, referentialConstraint.Annotations);

            return clone;
        }

        /// <summary>
        /// Clones the the specified data type.
        /// </summary>
        /// <param name="dataType">The data type to clone.</param>
        /// <returns>Cloned version of the data type.</returns>
        private static DataType Clone(this DataType dataType)
        {
            if (dataType == null)
            {
                return null;
            }

            var cloningVisitor = new CloningDataTypeVisitor();

            return cloningVisitor.Visit(dataType);
        }

        private static void CopyAnnotations(AnnotatedItem item, IList<Annotation> annotations)
        {
            foreach (var annotation in annotations)
            {
                var cloneableAnn = annotation as IDeepCloneableAnnotation;
                var newAnn = cloneableAnn == null ? annotation : cloneableAnn.DeepClone();
                item.Annotations.Add(newAnn);
            }
        }

        private static ComplexTypeReference NameOrNull(this ComplexType complexType)
        {
            return complexType != null ? new ComplexTypeReference(complexType.NamespaceName, complexType.Name) : null;
        }

        private static EntitySetReference NameOrNull(this EntitySet entitySet)
        {
            return entitySet != null ? new EntitySetReference(entitySet.Name) : null;
        }

        private static EntityTypeReference NameOrNull(this EntityType entityType)
        {
            return entityType != null ? new EntityTypeReference(entityType.NamespaceName, entityType.Name) : null;
        }

        private static TItem WithDataGeneratorInternal<TItem>(this TItem annotatedItem, IDataGenerator dataGenerator) where TItem : IAnnotatedItem
        {
            ExceptionUtilities.CheckArgumentNotNull(dataGenerator, "dataGenerator");

            DataGeneratorAnnotation dataGenAnnotation = annotatedItem.Annotations.OfType<DataGeneratorAnnotation>().SingleOrDefault();
            if (dataGenAnnotation != null)
            {
                annotatedItem.Annotations.Remove(dataGenAnnotation);
            }

            annotatedItem.Annotations.Add(new DataGeneratorAnnotation(dataGenerator));

            return annotatedItem;
        }

        private static TItem WithDataGenerationHintsInternal<TItem>(this TItem annotatedItem, params DataGenerationHint[] hints) where TItem : AnnotatedItem
        {
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(hints, "hints");

            if (hints.Length == 0)
            {
                return annotatedItem;
            }

            DataGenerationHintsAnnotation dataGenHintsAnnotation = annotatedItem.Annotations.OfType<DataGenerationHintsAnnotation>().SingleOrDefault();

            if (dataGenHintsAnnotation == null)
            {
                dataGenHintsAnnotation = new DataGenerationHintsAnnotation();
                annotatedItem.Annotations.Add(dataGenHintsAnnotation);
            }

            foreach (var hint in hints)
            {
                dataGenHintsAnnotation.Add(hint);
            }

            return annotatedItem;
        }

        /// <summary>
        /// A <see cref="DataType"/> visitor that clones the <see cref="DataType"/>.
        /// </summary>
        private sealed class CloningDataTypeVisitor : IDataTypeVisitor<DataType>
        {
            /// <summary>
            /// Visits the specified collection type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>A clone of the specified <see cref="DataType"/>.</returns>
            public DataType Visit(CollectionDataType dataType)
            {
                return new CollectionDataType()
                    .Nullable(dataType.IsNullable)
                    .WithElementDataType(dataType.ElementDataType.Accept(this));
            }

            /// <summary>
            /// Visits the specified complex type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>A clone of the specified <see cref="DataType"/>.</returns>
            public DataType Visit(ComplexDataType dataType)
            {
                return new ComplexDataType()
                   .WithDefinition(new ComplexTypeReference(dataType.Definition.NamespaceName, dataType.Definition.Name))
                   .Nullable(dataType.IsNullable);
            }

            /// <summary>
            /// Clones the specified <see cref="DataType"/>.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>A clone of the specified <see cref="DataType"/>.</returns>
            public DataType Visit(DataType dataType)
            {
                return dataType.Accept(this);
            }

            /// <summary>
            /// Visits the specified entity type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>A clone of the specified <see cref="DataType"/>.</returns>
            public DataType Visit(EntityDataType dataType)
            {
                return new EntityDataType().WithDefinition(new EntityTypeReference(dataType.Definition.NamespaceName, dataType.Definition.Name));
            }

            /// <summary>
            /// Visits the specified primitive type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>A clone of the specified <see cref="DataType"/>.</returns>
            public DataType Visit(PrimitiveDataType dataType)
            {
                EnumDataType enumDataType = dataType as EnumDataType;
                if (enumDataType != null)
                {
                    return enumDataType.WithDefinition(new EnumTypeReference(enumDataType.Definition.NamespaceName, enumDataType.Definition.Name));
                }
                else
                {
                    return dataType.Clone<PrimitiveDataType>();
                }
            }

            /// <summary>
            /// Visits the specified reference type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>A clone of the specified <see cref="DataType"/>.</returns>
            public DataType Visit(ReferenceDataType dataType)
            {
                return new ReferenceDataType()
                    .Nullable(dataType.IsNullable)
                    .WithEntityType(dataType.EntityType.NameOrNull());
            }
        }
    }
}
