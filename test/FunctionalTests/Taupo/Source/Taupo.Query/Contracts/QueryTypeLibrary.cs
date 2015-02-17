//---------------------------------------------------------------------
// <copyright file="QueryTypeLibrary.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Library that contains query types.
    /// </summary>
    public class QueryTypeLibrary
    {
        private Dictionary<string, QueryEntityType> queryEntityTypeCache = new Dictionary<string, QueryEntityType>();

        private IQueryEvaluationStrategy evaluationStrategy;
        private IPrimitiveDataTypeResolver storeDataTypeResolver;
        private IPrimitiveDataTypeResolver modelDataTypeResolver;

        private GetDefaultQueryTypeVisitor getDefaultQueryTypeVisitor;
        private Dictionary<EntityType, QueryEntityType> defaultQueryEntityTypes = new Dictionary<EntityType, QueryEntityType>();
        private Dictionary<ComplexType, QueryComplexType> defaultQueryComplexTypes = new Dictionary<ComplexType, QueryComplexType>();
        private Dictionary<EnumType, QueryScalarType> defaultQueryEnumTypes = new Dictionary<EnumType, QueryScalarType>();
        private Dictionary<SpatialDataType, QueryMappedScalarTypeWithStructure> defaultQuerySpatialTypes = new Dictionary<SpatialDataType, QueryMappedScalarTypeWithStructure>();
        private Dictionary<SpatialDataType, QueryClrSpatialType> defaultQueryClrSpatialTypes = new Dictionary<SpatialDataType, QueryClrSpatialType>();
        private Dictionary<Type, QueryScalarType> clrTypeToQueryScalarTypeMap = new Dictionary<Type, QueryScalarType>();
        private Dictionary<Type, PrimitiveDataType> clrTypeToPrimitiveDataTypeMap = new Dictionary<Type, PrimitiveDataType>();

        /// <summary>
        /// Initializes a new instance of the QueryTypeLibrary class.
        /// </summary>
        /// <param name="evaluationStrategy">The query evaluation strategy</param>
        public QueryTypeLibrary(IQueryEvaluationStrategy evaluationStrategy)
            : this(evaluationStrategy, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the QueryTypeLibrary class.
        /// </summary>
        /// <param name="strategy">The query evaluation strategy</param>
        /// <param name="storeTypeResolver">The store primitive type resolver</param>
        /// <param name="modelTypeResolver">The model primitive type resolver</param>
        public QueryTypeLibrary(IQueryEvaluationStrategy strategy, IPrimitiveDataTypeResolver storeTypeResolver, IPrimitiveDataTypeResolver modelTypeResolver)
        {
            this.evaluationStrategy = strategy;
            this.storeDataTypeResolver = storeTypeResolver;
            this.modelDataTypeResolver = modelTypeResolver;

            this.getDefaultQueryTypeVisitor = new GetDefaultQueryTypeVisitor(this);
        }

        /// <summary>
        /// Gets the root query type for a specified entity set
        /// </summary>
        /// <param name="entitySet">the entity set</param>
        /// <returns>the corresponding query type</returns>
        public QueryEntityType GetQueryEntityTypeForEntitySet(EntitySet entitySet)
        {
            return this.GetQueryEntityType(entitySet, entitySet.EntityType);
        }

        /// <summary>
        /// Gets the query type for a specified entity type within an entity set
        /// </summary>
        /// <param name="entitySet">the entity set</param>
        /// <param name="entityType">the entity type</param>
        /// <returns>the corresponding query type</returns>
        public virtual QueryEntityType GetQueryEntityType(EntitySet entitySet, EntityType entityType)
        {
            return this.queryEntityTypeCache[GetCacheKey(entitySet.Container, entitySet, entityType)];
        }
        
        /// <summary>
        /// Gets the default query type for a data type
        /// </summary>
        /// <param name="dataType">the data type</param>
        /// <returns>the default query type</returns>
        public QueryType GetDefaultQueryType(DataType dataType)
        {
            return this.getDefaultQueryTypeVisitor.GetDefaultQueryType(dataType);
        }

        /// <summary>
        /// Gets the default query scalar type for a Clr type
        /// </summary>
        /// <param name="clrType">the clr type</param>
        /// <returns>the default query scalar type</returns>
        public QueryScalarType GetDefaultQueryScalarType(Type clrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");

            QueryScalarType result;
            if (this.clrTypeToQueryScalarTypeMap.TryGetValue(clrType, out result))
            {
                return result;
            }
            
            PrimitiveDataType edmPrimitiveDataType;
            ExceptionUtilities.Assert(this.TryGetEdmDataType(clrType, out edmPrimitiveDataType), "Cannot resolve clr type '{0}' into primitive data type.", clrType.FullName);

            result = (QueryScalarType)this.GetDefaultQueryType(edmPrimitiveDataType);

            if (!this.clrTypeToQueryScalarTypeMap.ContainsKey(clrType))
            {
                this.clrTypeToQueryScalarTypeMap.Add(clrType, result);
            }
            else
            {
                var potentialDuplicate = this.clrTypeToQueryScalarTypeMap[clrType];
                ExceptionUtilities.Assert(object.ReferenceEquals(result, potentialDuplicate), "Duplicate query scalar type detected for clr type '{0}'. {1} != {2}", clrType.FullName, result, potentialDuplicate);
            }

            return result;
        }

        /// <summary>
        /// Updates the clr type mapping information with the given clr assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies that contain all the required clr type mapping information</param>
        /// <remarks>Currenlty oc mapping is based on full name match. We could have a separate component to do more than this in the future.</remarks>
        public void UpdateClrTypeMapping(IEnumerable<Assembly> assemblies)
        {
            var clrTypeLookup = new Dictionary<string, Type>();

            foreach (QueryEntityType queryEntityType in this.queryEntityTypeCache.Values)
            {
                queryEntityType.ClrType = this.GetClrType(queryEntityType.EntityType.FullName, clrTypeLookup, assemblies);
                this.UpdateClrTypeMappingForProperties(queryEntityType.Properties, clrTypeLookup, assemblies);
            }

            foreach (var enumScalarType in this.defaultQueryEnumTypes)
            {
                var queryClrEnum = enumScalarType.Value as QueryClrEnumType;

                if (queryClrEnum != null)
                {
                    queryClrEnum.ClrType = this.GetClrType(queryClrEnum.EnumType.FullName, clrTypeLookup, assemblies);
                }
            }
        }

        /// <summary>
        /// Gets all query enum types that have not been mapped.
        /// </summary>
        /// <returns>Query enum types that have not been mapped.</returns>
        internal IEnumerable<QueryClrEnumType> GetNonMappedEnumTypes()
        {
            return this.defaultQueryEnumTypes.Select(et => et.Value).OfType<QueryClrEnumType>();
        }

        /// <summary>
        /// Gets all query entity types
        /// </summary>
        /// <returns>All the query entity types in the library</returns>
        internal IEnumerable<QueryEntityType> GetQueryEntityTypes()
        {
            return this.queryEntityTypeCache.Values;
        }

        /// <summary>
        /// Sets query type for a given entity type
        /// </summary>
        /// <param name="entitySet">The entity set</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="queryType">The corresponding query type to be set</param>
        internal void SetQueryEntityType(EntitySet entitySet, EntityType entityType, QueryEntityType queryType)
        {
            this.queryEntityTypeCache[GetCacheKey(entitySet.Container, entitySet, entityType)] = queryType;
            this.defaultQueryEntityTypes[entityType] = queryType;
        }

        /// <summary>
        /// Sets query types for a given Enum Type
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="queryType">The corresponding query type.</param>
        internal void SetQueryEnumType(EnumType enumType, QueryScalarType queryType)
        {
            this.defaultQueryEnumTypes[enumType] = queryType;
        }

        /// <summary>
        /// Sets query type for a given compelx type
        /// </summary>
        /// <param name="complexType">The complex type</param>
        /// <param name="queryType">The corresponding query type to be set</param>
        internal void SetQueryComplexType(ComplexType complexType, QueryComplexType queryType)
        {
            this.defaultQueryComplexTypes[complexType] = queryType;
        }

        private static string GetCacheKey(EntityContainer container, EntitySet entitySet, EntityType entityType)
        {
            return container.Name + "." + entitySet.Name + "." + entityType.FullName;
        }

        private void UpdateClrTypeMappingForProperties(IEnumerable<QueryProperty> properties, Dictionary<string, Type> clrTypeLookup, IEnumerable<Assembly> assemblies)
        {
            var complexTypes = properties
                               .Select(p => p.PropertyType)
                               .Union(properties.Select(p => p.PropertyType)
                               .OfType<QueryCollectionType>()
                               .Select(c => c.ElementType))
                               .OfType<QueryComplexType>();

            foreach (QueryComplexType qct in complexTypes)
            {
                qct.ClrType = this.GetClrType(qct.ComplexType.FullName, clrTypeLookup, assemblies);
                this.UpdateClrTypeMappingForProperties(qct.Properties, clrTypeLookup, assemblies);
            }

            this.UpdateClrEnumTypePropertyMapping(properties, clrTypeLookup, assemblies);
        }

        private void UpdateClrEnumTypePropertyMapping(IEnumerable<QueryProperty> properties, Dictionary<string, Type> clrTypeLookup, IEnumerable<Assembly> assemblies)
        {
            var mappedEnumTypes = properties
                                .Select(p => p.PropertyType)
                                .Union(properties.Select(p => p.PropertyType)
                                .OfType<QueryCollectionType>()
                                .Select(c => c.ElementType))
                                .OfType<QueryMappedScalarType>()
                                .Where(e => e.ModelType is EnumDataType);

            foreach (QueryMappedScalarType qmet in mappedEnumTypes)
            {
                var edt = qmet.ModelType as EnumDataType;

                if (edt != null)
                {
                    var stubEnumType = this.defaultQueryEnumTypes[edt.Definition] as QueryClrEnumType;
                    if (stubEnumType != null)
                    {
                        // If we find a stub enum type, remove the stub and replace it with the mapped type.
                        this.defaultQueryEnumTypes.Remove(edt.Definition);
                        this.defaultQueryEnumTypes.Add(edt.Definition, qmet);
                    }
                    else
                    {
                        ExceptionUtilities.Assert(this.defaultQueryEnumTypes[edt.Definition] is QueryMappedScalarType, "All enum query types must either be mapped scalar types or Clr enum types.");
                    }

                    var clrType = this.GetClrType(edt.Definition.FullName, clrTypeLookup, assemblies);
                    qmet.ClrType = qmet.ModelType.IsNullable ? typeof(Nullable<>).MakeGenericType(clrType) : clrType;
               }
            }
        }

        private Type GetClrType(string fullName, Dictionary<string, Type> clrTypeLookup, IEnumerable<Assembly> assemblies)
        {
            if (clrTypeLookup.ContainsKey(fullName))
            {
                return clrTypeLookup[fullName];
            }

            foreach (Assembly assembly in assemblies)
            {
                Type type = assembly.GetType(fullName);
                if (type != null)
                {
                    clrTypeLookup[fullName] = type;
                    return type;
                }
            }

            return null;
        }

        private QueryScalarType GetDefaultQueryTypeForPrimitive(PrimitiveDataType primitiveType)
        {
            var enumDataType = primitiveType as EnumDataType;
            if (enumDataType != null)
            {
                return this.defaultQueryEnumTypes[enumDataType.Definition];
            }

            var spatialDataType = primitiveType as SpatialDataType;
            if (this.storeDataTypeResolver == null)
            {
                Type clrType = primitiveType.GetFacet<PrimitiveClrTypeFacet>().Value;
                if (primitiveType.IsNullable && clrType.IsValueType())
                {
                    clrType = typeof(Nullable<>).MakeGenericType(clrType);
                }

                if (spatialDataType != null)
                {
                    return this.GetDefaultQueryClrTypeForSpatial(spatialDataType, clrType);
                }

                return new QueryClrPrimitiveType(clrType, this.evaluationStrategy);
            }
            else
            {
                PrimitiveDataType storeType = this.storeDataTypeResolver.ResolvePrimitiveType(primitiveType);
                PrimitiveDataType modelType = this.modelDataTypeResolver.ResolvePrimitiveType(primitiveType);

                if (spatialDataType != null)
                {
                    return this.GetDefaultQueryTypeForSpatial(storeType, modelType, spatialDataType);
                }
                
                return new QueryMappedScalarType(modelType, storeType, this.evaluationStrategy);    
            }
        }

        private QueryClrSpatialType GetDefaultQueryClrTypeForSpatial(SpatialDataType spatialDataType, Type clrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(spatialDataType, "spatialDataType");
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");

            QueryClrSpatialType spatialClrType;
            if (this.defaultQueryClrSpatialTypes.TryGetValue(spatialDataType, out spatialClrType))
            {
                return spatialClrType;
            }

            spatialClrType = new QueryClrSpatialType(clrType, this.evaluationStrategy);
            this.defaultQueryClrSpatialTypes[spatialDataType] = spatialClrType;
            this.clrTypeToQueryScalarTypeMap[clrType] = spatialClrType;
            
            // wire-up inheritance based on CLR types
            var derivedTypes = clrType.GetAssembly().GetTypes().Where(t => t.IsSubclassOf(clrType)).ToList();
            foreach (var derivedType in derivedTypes)
            {
                // only add types which are in EDM
                PrimitiveDataType edmType;
                if (this.TryGetEdmDataType(derivedType, out edmType))
                {
                    spatialClrType.DerivedTypes.Add((QueryClrSpatialType)this.GetDefaultQueryScalarType(derivedType));
                }
            }

            spatialClrType.Add(spatialDataType.Properties.Select(p => QueryProperty.Create(p.Name, this.GetDefaultQueryType(p.PropertyType))));
            spatialClrType.Add(spatialDataType.Methods);
           
            return spatialClrType.MakeReadOnly();
        }

        private QueryScalarType GetDefaultQueryTypeForSpatial(PrimitiveDataType storeType, PrimitiveDataType modelType, SpatialDataType spatialDataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(storeType, "storeType");
            ExceptionUtilities.CheckArgumentNotNull(modelType, "modelType");
            ExceptionUtilities.CheckArgumentNotNull(spatialDataType, "spatialDataType");

            QueryMappedScalarTypeWithStructure spatialQDT;
            if (this.defaultQuerySpatialTypes.TryGetValue(spatialDataType, out spatialQDT))
            {
                return spatialQDT;
            }

            SpatialDataType spatialStoreType = storeType as SpatialDataType;
            SpatialDataType spatialModelType = modelType as SpatialDataType;

            var spatialMappedType = new QueryMappedScalarTypeWithStructure(spatialModelType, spatialStoreType, this.evaluationStrategy);
            this.defaultQuerySpatialTypes[spatialDataType] = spatialMappedType;

            spatialMappedType.Add(spatialModelType.Properties.Select(p => QueryProperty.Create(p.Name, this.GetDefaultQueryType(p.PropertyType))));
            spatialMappedType.Add(spatialModelType.Methods);
            
            return spatialMappedType.MakeReadOnly();
        }

        private QueryEntityType GetDefaultQueryTypeForEntity(EntityDataType entityDataType)
        {
            EntityType entityType = entityDataType.Definition;
            return this.defaultQueryEntityTypes[entityType];
        }

        private QueryComplexType GetDefaultQueryTypeForComplex(ComplexDataType complexDataType)
        {
            ComplexType complexType = complexDataType.Definition;
            return this.defaultQueryComplexTypes[complexType];
        }

        private QueryReferenceType GetDefaultQueryTypeForReference(ReferenceDataType referenceDataType)
        {
            ExceptionUtilities.CheckObjectNotNull(referenceDataType.EntityType, "Cannot determine QueryType for Reference with no EntityType");
            return this.defaultQueryEntityTypes[referenceDataType.EntityType].CreateReferenceType();
        }

        private PrimitiveDataType TryResolveConceptualSpatialDataType(SpatialDataType spatialDataType)
        {
            PrimitiveDataType resolved = null;

            if (this.modelDataTypeResolver != null)
            {
                try
                {
                    resolved = this.modelDataTypeResolver.ResolvePrimitiveType(spatialDataType);
                }
                catch (TaupoInvalidOperationException)
                {
                }
                catch (TaupoNotSupportedException)
                {
                }
            }

            return resolved;
        }

        private bool DoesClrTypeMatch(PrimitiveDataType primitiveDataType, Type clrType)
        {
            var clrTypeFromDataType = primitiveDataType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
            return clrType == clrTypeFromDataType;
        }

        private bool TryGetEdmDataType(Type clrType, out PrimitiveDataType edmPrimitiveDataType)
        {
            if (this.clrTypeToPrimitiveDataTypeMap.TryGetValue(clrType, out edmPrimitiveDataType))
            {
                return true;
            }

            // TODO: should query type library be aware of Edm version? Use latest for now.
            foreach (var t in EdmDataTypes.GetAllPrimitiveTypes(EdmVersion.Latest))
            {
                var primitiveDataType = t;
                var spatialDataType = primitiveDataType as SpatialDataType;
                if (spatialDataType != null)
                {
                    primitiveDataType = this.TryResolveConceptualSpatialDataType(spatialDataType);
                    if (primitiveDataType == null)
                    {
                        continue;
                    }
                }

                if (this.DoesClrTypeMatch(primitiveDataType, clrType))
                {
                    this.clrTypeToPrimitiveDataTypeMap[clrType] = primitiveDataType;
                    edmPrimitiveDataType = primitiveDataType;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Visitor to get default query type from a given data type (in model)
        /// </summary>
        private class GetDefaultQueryTypeVisitor : IDataTypeVisitor<QueryType>
        {
            private QueryTypeLibrary parent;

            /// <summary>
            /// Initializes a new instance of the GetDefaultQueryTypeVisitor class.
            /// </summary>
            /// <param name="parent">the parent query type library</param>
            public GetDefaultQueryTypeVisitor(QueryTypeLibrary parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Gets the default query type from a given data type (in model)
            /// </summary>
            /// <param name="dataType">The given data type.</param>
            /// <returns>The default query type.</returns>
            public QueryType GetDefaultQueryType(DataType dataType)
            {
                return dataType.Accept(this);
            }

            /// <summary>
            /// Visits the specified collection type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the corresponding default query type.</returns>
            public QueryType Visit(CollectionDataType dataType)
            {
                QueryType elementType = this.GetDefaultQueryType(dataType.ElementDataType);
                return elementType.CreateCollectionType();
            }

            /// <summary>
            /// Visits the specified complex type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the corresponding default query type.</returns>
            public QueryType Visit(ComplexDataType dataType)
            {
                return this.parent.GetDefaultQueryTypeForComplex(dataType);
            }

            /// <summary>
            /// Visits the specified entity type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the corresponding default query type.</returns>
            public QueryType Visit(EntityDataType dataType)
            {
                return this.parent.GetDefaultQueryTypeForEntity(dataType);
            }

            /// <summary>
            /// Visits the specified primitive type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the corresponding default query type.</returns>
            public QueryType Visit(PrimitiveDataType dataType)
            {
                return this.parent.GetDefaultQueryTypeForPrimitive(dataType);
            }

            /// <summary>
            /// Visits the specified reference type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>the corresponding default query type.</returns>
            public QueryType Visit(ReferenceDataType dataType)
            {
                return this.parent.GetDefaultQueryTypeForReference(dataType);
            }
        }
    }
}
