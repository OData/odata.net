//---------------------------------------------------------------------
// <copyright file="QueryDataSetBuilderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Base class for EntityContainerData-based implementations of <see cref="IQueryDataSetBuilder"/>.
    /// Converts <see cref="EntityContainerData" /> into <see cref="QueryDataSet" />.
    /// </summary>
    public abstract class QueryDataSetBuilderBase : IQueryDataSetBuilder
    {
        private Dictionary<string, Dictionary<EntityDataKey, QueryStructuralValue>> rowInstances;
        private IDictionary<string, QueryStructuralType> rootDataTypes;

        /// <summary>
        /// Builds the <see cref="QueryDataSet"/> from the specified container data and query queryRepository.
        /// </summary>
        /// <param name="rootDataTypeMap">The collection of rootDataTypes used to build the data set.</param>
        /// <param name="entityContainerData">Entity Container Data</param>
        /// <returns>
        /// Instance of <see cref="QueryDataSet"/> with data populated from the containerData
        /// </returns>
        public virtual IQueryDataSet Build(IDictionary<string, QueryStructuralType> rootDataTypeMap, EntityContainerData entityContainerData)
        {
            QueryDataSet dataSet = new QueryDataSet();

            this.rootDataTypes = rootDataTypeMap;
            this.rowInstances = new Dictionary<string, Dictionary<EntityDataKey, QueryStructuralValue>>();

            if (null != entityContainerData)
            {
                // phase 1 - build stub QueryStructuralValues for each EntityDataSetRow
                // and create result collections. We're doing it in two phases so that we
                // can set up relationships more easily since all objects
                // are guaranteed to exist.
                foreach (var entitySet in entityContainerData.EntityContainer.EntitySets)
                {
                    var entitySetData = entityContainerData.GetEntitySetData(entitySet.Name);
                    var collection = this.BuildStubEntities(entitySetData);

                    dataSet.RootQueryData[entitySet.Name] = collection;
                }

                // phase 2 - copy actual data into pre-generated objects
                foreach (var entitySet in entityContainerData.EntityContainer.EntitySets)
                {
                    var entitySetData = entityContainerData.GetEntitySetData(entitySet.Name);

                    this.PopulateObjectInstances(entitySetData);
                }
            }

            return dataSet;
        }

        /// <summary>
        /// Initializes the given queryType
        /// </summary>
        /// <param name="queryType">the query type to initialize</param>
        /// <returns>the query structurval value</returns>
        protected virtual QueryStructuralValue InitializeEntityValue(QueryEntityType queryType)
        {
            return queryType.CreateNewInstance();
        }

        /// <summary>
        /// Initializes query collection values.
        /// </summary>
        /// <param name="entitySetData">entity set data</param>
        /// <returns>initial query collection value</returns>
        protected virtual QueryCollectionValue BuildStubEntities(EntitySetData entitySetData)
        {
            string entitySetName = entitySetData.EntitySet.Name;

            var elements = new List<QueryStructuralValue>();
            var setRowMap = new Dictionary<EntityDataKey, QueryStructuralValue>();

            foreach (EntitySetDataRow row in entitySetData.Rows)
            {
                var queryType = this.GetQueryType(entitySetData.EntitySet.Name, row.EntityType);
                var instance = this.InitializeEntityValue(queryType);

                setRowMap[row.Key] = instance;
                elements.Add(instance);
            }

            this.rowInstances[entitySetName] = setRowMap;

            var rootElementType = this.GetQueryType(entitySetName, entitySetData.EntitySet.EntityType);

            return QueryCollectionValue.Create(rootElementType, elements.ToArray());
        }

        /// <summary>
        /// Helper method for populating property values on the given instance
        /// </summary>
        /// <param name="row">The row containing the data</param>
        /// <param name="instance">The structural instacne</param>
        protected virtual void PopulateInstanceFromRow(EntitySetDataRow row, QueryStructuralValue instance)
        {
            this.PopulateNavigations(row, instance);
            this.PopulateScalarAndComplexCollectionAndNonCollectionProperties(row, instance);
        }

        private static string CreateQueryPropertyList(IList<QueryProperty> memberProperties)
        {
            return CreatePropertyList<QueryProperty>(memberProperties, qp => qp.Name);
        }

        private static string CreateMemberPropertyList(IList<MemberProperty> memberProperties)
        {
            return CreatePropertyList<MemberProperty>(memberProperties, qp => qp.Name);
        }

        private static string CreatePropertyList<T>(IList<T> memberProperties, Func<T, string> getMemberName)
        {
            string properties = null;

            foreach (T p in memberProperties)
            {
                if (properties == null)
                {
                    properties = getMemberName(p);
                }
                else
                {
                    properties = properties + ", " + getMemberName(p);
                }
            }

            return properties;
        }

        private void PopulateObjectInstances(EntitySetData entitySetData)
        {
            foreach (var row in entitySetData.Rows)
            {
                var instance = this.rowInstances[entitySetData.EntitySet.Name][row.Key];

                this.PopulateInstanceFromRow(row, instance);
            }
        }

        private void PopulateScalarAndComplexCollectionAndNonCollectionProperties(EntitySetDataRow row, QueryStructuralValue instance)
        {
            IEnumerable<QueryProperty> allProperties = instance.Type.Properties;

            if (instance.Type is QueryEntityType)
            {
                List<string> properties = ((QueryEntityType)instance.Type).EntityType.AllProperties.Select(p => p.Name).ToList();
                allProperties = instance.Type.Properties.Where(m => properties.Contains(m.Name));
            }

            List<QueryProperty> nonEntityQueryProperties = new List<QueryProperty>();
            foreach (QueryProperty qp in allProperties)
            {
                nonEntityQueryProperties.Add(qp);
            }

            this.BuildStructuralPropertiesQueryValue(instance, string.Empty, row.EntityType.AllProperties.ToList(), nonEntityQueryProperties, row);
        }

        private QueryValue BuildCollectionQueryValue(string propertyPath, QueryCollectionType queryCollectionType, EntitySetDataRow row)
        {
            QueryScalarType scalarElementDataType = queryCollectionType.ElementType as QueryScalarType;
            QueryComplexType complexTypeElementDataType = queryCollectionType.ElementType as QueryComplexType;
            List<QueryValue> queryValues = new List<QueryValue>();
            if (scalarElementDataType != null)
            {
                int i = 0;
                while (row.PropertyPaths.Any(pp => pp == propertyPath + i))
                {
                    var value = row[propertyPath + i];
                    queryValues.Add(scalarElementDataType.CreateValue(value));
                    i++;
                }
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(complexTypeElementDataType, "PropertyPath '{0}' is an invalid type '{1}'", propertyPath, queryCollectionType.ElementType);

                int i = 0;
                while (row.PropertyPaths.Where(pp => pp.StartsWith(propertyPath + i, StringComparison.Ordinal)).Count() > 0)
                {
                    QueryStructuralValue complexChildInstance = complexTypeElementDataType.CreateNewInstance();
                    this.BuildStructuralPropertiesQueryValue(complexChildInstance, propertyPath + i + ".", complexTypeElementDataType.ComplexType.Properties, complexTypeElementDataType.Properties, row);
                    queryValues.Add(complexChildInstance);
                    i++;
                }
            }

            return queryCollectionType.CreateCollectionWithValues(queryValues.ToArray());
        }

        private void BuildStructuralPropertiesQueryValue(QueryStructuralValue instance, string propertyPath, IList<MemberProperty> properties, IList<QueryProperty> queryProperties, EntitySetDataRow row)
        {
            ExceptionUtilities.Assert(properties.Count == queryProperties.Count, "QueryProperties '{0}' and MemberProperties '{1}' are not the same number!", CreateQueryPropertyList(queryProperties), CreateMemberPropertyList(properties));

            // TODO: Some Taupo framework pieces skip over StreamDataType properties
            foreach (MemberProperty childProperty in properties.Where(p => !(p.PropertyType is StreamDataType)))
            {
                string childPropertyPath = propertyPath + childProperty.Name;
                List<QueryProperty> childQueryProperties = queryProperties.Where(p => p.Name == childProperty.Name).ToList();
                ExceptionUtilities.Assert(childQueryProperties.Count == 1, "Could not find query property based on MemberProperty Name '{0}' in list of query properties '{1}'", childProperty.Name, CreateQueryPropertyList(childQueryProperties));

                QueryProperty childQueryProperty = childQueryProperties.First();

                QueryCollectionType childCollectionDataType = childQueryProperty.PropertyType as QueryCollectionType;
                QueryScalarType childScalarType = childQueryProperty.PropertyType as QueryScalarType;
                QueryComplexType childComplexType = childQueryProperty.PropertyType as QueryComplexType;

                if (childCollectionDataType != null)
                {
                    instance.SetValue(childProperty.Name, this.BuildCollectionQueryValue(childPropertyPath + ".", childCollectionDataType, row));
                }
                else if (childScalarType != null)
                {
                    var value = row[childPropertyPath];
                    var queryValue = childScalarType.CreateValue(value);
                    instance.SetValue(childQueryProperty.Name, queryValue);
                }
                else
                {
                    ExceptionUtilities.CheckObjectNotNull(childComplexType, "Unknown type '{0}'", childProperty.PropertyType);

                    // If a complex type instance is null in the datarow, we will create a QueryStructuralValue indicating null and set it on the instance.
                    if (row.PropertyPaths.Contains(childPropertyPath) && row[childPropertyPath] == null)
                    {
                        instance.SetValue(childProperty.Name, new QueryStructuralValue(childComplexType, true, null, childComplexType.EvaluationStrategy));
                    }
                    else
                    {
                        QueryStructuralValue childInstance = childComplexType.CreateNewInstance();
                        this.BuildStructuralPropertiesQueryValue(childInstance, childPropertyPath + ".", childComplexType.ComplexType.Properties, childComplexType.Properties, row);
                        instance.SetValue(childProperty.Name, childInstance);
                    }
                }
            }
        }

        private void PopulateNavigations(EntitySetDataRow row, QueryStructuralValue instance)
        {
            var entityInstance = instance as QueryEntityValue;
            EntitySetData entitySetData = row.Parent;
            EntityContainerData containerData = entitySetData.Parent;

            foreach (AssociationSet associationSet in containerData.EntityContainer.AssociationSets)
            {
                foreach (var fromSetEnd in associationSet.Ends.Where(e => e.EntitySet == entitySetData.EntitySet))
                {
                    var fromEntityType = fromSetEnd.AssociationEnd.EntityType;
                    if (!row.EntityType.IsKindOf(fromEntityType))
                    {
                        continue;
                    }

                    var toSetEnd = associationSet.GetOtherEnd(fromSetEnd);
                    
                    this.PopulateNavigateResult(row, entityInstance, associationSet, fromSetEnd, toSetEnd);

                    // if Navigation property exists, populate it as well
                    var navProp = row.EntityType.AllNavigationProperties.SingleOrDefault(p => p.Association == associationSet.AssociationType &&                                                                                          
                                                                                          p.ToAssociationEnd == toSetEnd.AssociationEnd);
                    if (navProp != null)
                    {
                        instance.SetValue(navProp.Name, entityInstance.GetNavigateResult(navProp.Association, navProp.ToAssociationEnd));
                    }
                }
            }
        }

        private void PopulateNavigateResult(EntitySetDataRow row, QueryEntityValue entityInstance, AssociationSet associationSet, AssociationSetEnd fromSetEnd, AssociationSetEnd toSetEnd)
        {
            EntityContainerData containerData = row.Parent.Parent;
            var associationSetData = containerData.GetAssociationSetData(associationSet.Name);

            var targetInstanceLookup = this.rowInstances[toSetEnd.EntitySet.Name];

            var associatedObjects = associationSetData.Rows
                .Where(r => r.GetRoleKey(fromSetEnd.AssociationEnd.RoleName).Equals(row.Key))
                .Select(r => targetInstanceLookup[r.GetRoleKey(toSetEnd.AssociationEnd.RoleName)])
                .ToArray();

            var toEntityType = toSetEnd.AssociationEnd.EntityType;
            var toQueryEntityType = this.GetQueryType(toSetEnd.EntitySet.Name, toEntityType);
            if (toSetEnd.AssociationEnd.Multiplicity == EndMultiplicity.Many)
            {
                var collectionType = toQueryEntityType.CreateCollectionType();
                var collectionValue = collectionType.CreateCollectionWithValues(associatedObjects);
                entityInstance.SetNavigateResult(associationSet.AssociationType, toSetEnd.AssociationEnd, collectionValue);
            }
            else
            {
                if (associatedObjects.Length == 0)
                {
                    entityInstance.SetNavigateResult(associationSet.AssociationType, toSetEnd.AssociationEnd, toQueryEntityType.NullValue);
                }
                else
                {
                    if (associatedObjects.Length != 1)
                    {                       
                        var debugAssociatedValues = string.Join("," + Environment.NewLine, associatedObjects.Select(ao => ao.ToString()));
                        throw new TaupoInfrastructureException(
                            string.Format(
                                CultureInfo.InvariantCulture, 
                                "Found {0} associated objects for {1}.{2}, on Entity Instance {3} associated Objects = {4}", 
                                associatedObjects.Length, 
                                associationSet.AssociationType.FullName, 
                                fromSetEnd.AssociationEnd.RoleName, 
                                entityInstance, 
                                debugAssociatedValues));
                    }
                    
                    var targetInstance = associatedObjects.Single();
                    entityInstance.SetNavigateResult(associationSet.AssociationType, toSetEnd.AssociationEnd, targetInstance);
                }
            }
        }

        private QueryEntityType GetQueryType(string entitySetName, EntityType entityType)
        {
            var queryEntityType = this.rootDataTypes[entitySetName] as QueryEntityType;
            ExceptionUtilities.Assert(queryEntityType != null, "Repository must use QueryEntityTypes.");

            if (queryEntityType.EntityType == entityType)
            {
                return queryEntityType;
            }

            return queryEntityType.DerivedTypes.OfType<QueryEntityType>().Where(et => et.EntityType == entityType).Single();
        }
    }
}