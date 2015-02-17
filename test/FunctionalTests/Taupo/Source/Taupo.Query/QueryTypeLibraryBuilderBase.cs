//---------------------------------------------------------------------
// <copyright file="QueryTypeLibraryBuilderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Base implementation to build a query type library
    /// </summary>
    public abstract class QueryTypeLibraryBuilderBase : IQueryTypeLibraryBuilder
    {
        /// <summary>
        /// Initializes a new instance of the QueryTypeLibraryBuilderBase class.
        /// </summary>
        /// <param name="evaluationStrategy">The query evaluation strategy</param>
        protected QueryTypeLibraryBuilderBase(IQueryEvaluationStrategy evaluationStrategy)
        {
            this.EvaluationStrategy = evaluationStrategy;
        }

        /// <summary>
        /// Gets the query evaluation strategy
        /// </summary>
        protected IQueryEvaluationStrategy EvaluationStrategy { get; private set; }

        /// <summary>
        /// Builds the query type library with out Clr type mapping information, given the entity model  
        /// </summary>
        /// <param name="model">The given entity model</param>
        /// <returns>The query type library</returns>
        public virtual QueryTypeLibrary BuildLibraryWithoutClrTypeMapping(EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            QueryTypeLibrary lib = this.CreateInitialQueryTypeLibrary();

            foreach (var enumType in model.EnumTypes)
            {
                var stubEnumType = this.CreateStubEnumType(enumType);
                lib.SetQueryEnumType(enumType, stubEnumType);
            }
            
            foreach (var container in model.EntityContainers)
            {
                this.CreateStubEntityTypes(lib, container);
                this.BuildStructuralTypes(lib, container);
            }

            return lib;
        }

        /// <summary>
        /// Create the initial query type library
        /// </summary>
        /// <returns>The initial query type library</returns>
        /// <remarks>The default implementation does not have store semantics. Derived class should override as appropriate.</remarks>
        protected virtual QueryTypeLibrary CreateInitialQueryTypeLibrary()
        {
            return new QueryTypeLibrary(this.EvaluationStrategy);
        }

        /// <summary>
        /// Get the query type for a scalar property (with mapped store semantics, if applicable)
        /// </summary>
        /// <param name="pathToProperty">The path to find the property</param>
        /// <param name="library">The query type library</param>
        /// <param name="modelType">The model data type for the property</param>
        /// <returns>The query scalar type for the property</returns>
        /// <remarks>Default implementation just returns default type (meaning no special store semantics from mapping). Derived class should override as appropriate.</remarks>
        protected virtual QueryScalarType GetQueryTypeForMappedProperty(PathToProperty pathToProperty, QueryTypeLibrary library, PrimitiveDataType modelType)
        {
            return (QueryScalarType)library.GetDefaultQueryType(modelType);
        }

        /// <summary>
        /// Creates a QueryComplexType using a ComplexType
        /// </summary>
        /// <param name="complexType">ComplexType to create Query Type Stub</param>
        /// <returns>Returns a Query Complex Type</returns>
        protected QueryComplexType CreateStubComplexType(ComplexType complexType)
        {
            return new QueryComplexType(this.EvaluationStrategy, complexType, null);
        }

        /// <summary>
        /// Creates a QueryProperty for a property that is a non entity Collection 
        /// </summary>
        /// <param name="library">Library Query Type</param>
        /// <param name="result">resulting Query Structural Type</param>
        /// <param name="collectionProperty">Member to calculate</param>
        /// <param name="pathToProperty">Path to the Property</param>
        /// <returns>A Query Property of the collectionType</returns>
        protected virtual QueryProperty CreateNonentityCollectionMember(QueryTypeLibrary library, QueryStructuralType result, MemberProperty collectionProperty, PathToProperty pathToProperty)
        {
            throw new TaupoNotSupportedException("This type of property is not supported.");
        }

        /// <summary>
        /// Creates new members on a structural type
        /// </summary>
        /// <param name="library">Type library</param>
        /// <param name="result">Structural Type to add members to</param>
        /// <param name="properties">Properties of the Structural Type member</param>
        /// <param name="pathToProperty">Path to the Property</param>
        protected void CreateMembers(QueryTypeLibrary library, QueryStructuralType result, IEnumerable<MemberProperty> properties, PathToProperty pathToProperty)
        {
            // TODO: Some Taupo framework pieces skip over StreamDataType properties
            foreach (var prop in properties.Where(p => !(p.PropertyType is StreamDataType)))
            {
                QueryProperty queryProperty = null;
                pathToProperty.PathStackWithinEntityType.Add(prop.Name);

                var pdt = prop.PropertyType as PrimitiveDataType;
                var cdt = prop.PropertyType as ComplexDataType;
                if (pdt != null)
                {
                    QueryScalarType queryPropertyType = this.GetQueryTypeForMappedProperty(pathToProperty, library, pdt);
                    queryProperty = QueryProperty.Create(prop.Name, queryPropertyType);
                }
                else if (cdt != null)
                {
                    ComplexType ct = cdt.Definition;
                    QueryComplexType queryComplexType = this.CreateStubComplexType(ct);
                    this.CreateMembers(library, queryComplexType, ct.Properties, pathToProperty);
                    library.SetQueryComplexType(ct, queryComplexType);
                    queryProperty = QueryProperty.Create(prop.Name, queryComplexType);
                }
                else
                {
                    queryProperty = this.CreateNonentityCollectionMember(library, result, prop, pathToProperty);
                }

                pathToProperty.PathStackWithinEntityType.RemoveAt(pathToProperty.PathStackWithinEntityType.Count - 1);

                queryProperty.SetPrimaryKey(prop.IsPrimaryKey);
                result.Add(queryProperty);
            }
        }

        private void CreateStubEntityTypes(QueryTypeLibrary library, EntityContainer container)
        {
            foreach (var entitySet in container.EntitySets)
            {
                var rootType = entitySet.EntityType;
                var allDerivedTypes = container.Model.EntityTypes.Where(et => et.IsKindOf(rootType));

                foreach (var entityType in allDerivedTypes)
                {
                    QueryEntityType stub = this.CreateStubEntityType(entityType, entitySet);
                    library.SetQueryEntityType(entitySet, entityType, stub);
                }

                // set up Parent for each type
                foreach (var childType in allDerivedTypes.Where(et => et != rootType))
                {
                    library.GetQueryEntityType(entitySet, childType).Parent = library.GetQueryEntityType(entitySet, childType.BaseType);
                }
            }

            // TODO: maybe this is wrong if >1 containers! Add unit test and fix.
            // construct DerivedTypes for each type
            foreach (var type in library.GetQueryEntityTypes())
            {
                for (var parent = type.Parent; parent != null; parent = parent.Parent)
                {
                    parent.DerivedTypes.Add(type);
                }
            }
        }

        private QueryScalarType CreateStubEnumType(EnumType enumType)
        {
            return new QueryClrEnumType(this.EvaluationStrategy, enumType, null);
        }

        private QueryEntityType CreateStubEntityType(EntityType entityType, EntitySet entitySet)
        {
            return new QueryEntityType(this.EvaluationStrategy, entityType, entitySet, null);
        }

        private void BuildStructuralTypes(QueryTypeLibrary library, EntityContainer container)
        {
            foreach (var entitySet in container.EntitySets)
            {
                var rootType = entitySet.EntityType;
                var allDerivedTypes = container.Model.EntityTypes.Where(et => et.IsKindOf(rootType));

                foreach (var entityType in allDerivedTypes)
                {
                    var queryEntityType = library.GetQueryEntityType(entitySet, entityType);

                    var pathToProperty = new PathToProperty(container, entitySet, entityType);
                    this.CreateMembers(library, queryEntityType, entityType.AllProperties, pathToProperty);
                    ExceptionUtilities.Assert(pathToProperty.PathStackWithinEntityType.Count == 0, "Path to property stack is not empty.");

                    this.CreateNavigationMembers(library, queryEntityType, entityType.AllNavigationProperties, container);
                }
            }
        }

        private void CreateNavigationMembers(QueryTypeLibrary library, QueryEntityType result, IEnumerable<NavigationProperty> navigationProperties, EntityContainer container)
        {
            foreach (var navprop in navigationProperties)
            {
                // handle MEST scenario where there are multiple association sets corresponding to a navigation property
                var asets = container.AssociationSets.Where(c => c.AssociationType == navprop.Association);
                var aset = asets.Single(set => set.Ends.Any(end => end.AssociationEnd == navprop.FromAssociationEnd && end.EntitySet == result.EntitySet));

                var toSet = aset.Ends.Single(e => e.AssociationEnd == navprop.ToAssociationEnd).EntitySet;
                var targetEntityType = library.GetQueryEntityType(toSet, navprop.ToAssociationEnd.EntityType);

                QueryProperty property;

                if (navprop.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
                {
                    // collection property
                    property = QueryProperty.Create(navprop.Name, targetEntityType.CreateCollectionType());
                }
                else
                {
                    // reference property
                    property = QueryProperty.Create(navprop.Name, targetEntityType);
                }

                result.Add(property);
            }
        }

        /// <summary>
        /// Class that have the necessary information to find a property instance (to retrieve mapping information)
        /// </summary>
        protected class PathToProperty
        {
            /// <summary>
            /// Initializes a new instance of the PathToProperty class.
            /// </summary>
            /// <param name="container">The entity container</param>
            /// <param name="entitySet">The entity set</param>
            /// <param name="entityType">The entity type</param>
            public PathToProperty(EntityContainer container, EntitySet entitySet, EntityType entityType)
            {
                this.Container = container;
                this.EntitySet = entitySet;
                this.EntityType = entityType;
                this.PathStackWithinEntityType = new List<string>();
            }

            /// <summary>
            /// Gets the entity container
            /// </summary>
            public EntityContainer Container { get; private set; }

            /// <summary>
            /// Gets the entity set
            /// </summary>
            public EntitySet EntitySet { get; private set; }

            /// <summary>
            /// Gets the entity type
            /// </summary>
            public EntityType EntityType { get; private set; }

            /// <summary>
            /// Gets the path to property within the entity type
            /// </summary>
            public IList<string> PathStackWithinEntityType { get; private set; }
        }
    }
}
