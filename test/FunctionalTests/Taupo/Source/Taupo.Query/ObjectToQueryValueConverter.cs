//---------------------------------------------------------------------
// <copyright file="ObjectToQueryValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Converts the given object into QueryValue
    /// </summary>
    [ImplementationName(typeof(IObjectToQueryValueConverter), "Default")]
    public class ObjectToQueryValueConverter : IObjectToQueryValueConverter, IQueryTypeVisitor<QueryValue>
    {
        private Dictionary<object, QueryStructuralValue> visitedStructurals;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency(IsRequired = true)]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets the stack of elements from which the result QueryValue is created.
        /// </summary>
        protected Stack<object> ResultFragmentStack { get; private set; }

        /// <summary>
        /// Converts the given object into QueryValue.
        /// </summary>
        /// <param name="value">Object that will be converted.</param>
        /// <param name="queryType">QueryType of the result.</param>
        /// <returns>QueryValue representing the given object.</returns>
        public QueryValue ConvertToQueryValue(object value, QueryType queryType)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryType, "queryType");

            this.visitedStructurals = new Dictionary<object, QueryStructuralValue>();
            this.ResultFragmentStack = new Stack<object>();

            var queryableValue = value as IQueryable;
            if (queryableValue != null)
            {
                this.Logger.WriteLine(LogLevel.Verbose, "Executing query: " + queryableValue.Expression);
            }

            return this.Convert(value, queryType);
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryAnonymousStructuralType type)
        {
            var resultFragment = this.ResultFragmentStack.Pop();
            
            return this.PopulateProperties(type.CreateNewInstance(), resultFragment);
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryClrEnumType type)
        {
            return this.VisitScalar(type);
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryClrPrimitiveType type)
        {
            return this.VisitScalar(type);
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryCollectionType type)
        {
            var resultFragment = this.ResultFragmentStack.Pop();
            var resultCollectionElements = new List<QueryValue>();

            // need to materialize results first to get the complete graph, before we start converting to QueryValue
            var resultFragmentList = ((IEnumerable)resultFragment).Cast<object>().ToList();
            foreach (var resultFragmentElement in resultFragmentList)
            {
                var resultCollectionElement = this.Convert(resultFragmentElement, type.ElementType);
                resultCollectionElements.Add(resultCollectionElement);
            }

            return type.CreateCollectionWithValues(resultCollectionElements);
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryComplexType type)
        {
            var resultFragment = this.ResultFragmentStack.Pop();
            
            QueryStructuralValue visitedComplexType;
            if (this.visitedStructurals.TryGetValue(resultFragment, out visitedComplexType))
            {
                return visitedComplexType;
            }

            ExceptionUtilities.Assert(
                resultFragment.GetType().Name == type.ComplexType.Name, 
                "Names don't match. Expecting: " + type.ComplexType.Name + ". Actual: " + resultFragment.GetType().Name + ".");

            var resultComplexType = type.CreateNewInstance();
            this.visitedStructurals[resultFragment] = resultComplexType;
            resultComplexType = this.PopulateProperties(resultComplexType, resultFragment);

            return resultComplexType;
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryEntityType type)
        {
            var resultFragment = this.ResultFragmentStack.Pop();

            QueryStructuralValue visitedEntity;
            if (this.visitedStructurals.TryGetValue(resultFragment, out visitedEntity))
            {
                return visitedEntity;
            }

            // taking inheritance into account - while 'expected' query entity type is Product, the actual materialized object can be DiscontinuedProduct 
            var entityTypeHierarchy = new[] { type }.Concat(type.DerivedTypes).Cast<QueryEntityType>();
            var matchingEntityType = entityTypeHierarchy.Where(et => et.EntityType.Name == resultFragment.GetType().Name).SingleOrDefault();
            ExceptionUtilities.CheckObjectNotNull(matchingEntityType, "Could not find " + typeof(QueryEntityType).Name + " that would match the given type: " + resultFragment.GetType().Name);

            var resultEntity = matchingEntityType.CreateNewInstance();
            this.visitedStructurals[resultFragment] = resultEntity;
            resultEntity = this.PopulateProperties(resultEntity, resultFragment);

            return resultEntity;
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryGroupingType type)
        {
            var resultFragment = this.ResultFragmentStack.Pop();
            var groupingInterface = resultFragment.GetType().GetInterfaces().Where(i => i.IsGenericType() && i.GetGenericTypeDefinition() == typeof(IGrouping<,>)).SingleOrDefault();
            ExceptionUtilities.CheckObjectNotNull(groupingInterface, "Expecting result fragment to be a grouping.");

            Type keyType = groupingInterface.GetGenericArguments()[0];
            Type elementType = groupingInterface.GetGenericArguments()[1];
            var generateGroupingMethod = typeof(ObjectToQueryValueConverter).GetMethods(false, false).Where(m => m.Name == "GenerateGroupingItem" && m.IsGenericMethod).Single();
            generateGroupingMethod = generateGroupingMethod.MakeGenericMethod(keyType, elementType);

            var resultGrouping = (QueryStructuralValue)generateGroupingMethod.Invoke(this, new[] { resultFragment, type });

            return resultGrouping;
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryMappedScalarType type)
        {
            return this.VisitScalar(type);
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public QueryValue Visit(QueryMappedScalarTypeWithStructure type)
        {
            return this.VisitScalar(type);
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public virtual QueryValue Visit(QueryRecordType type)
        {
            throw new TaupoNotSupportedException("Not supported by generic converter. Use Entity Framework specific implementation.");
        }

        /// <summary>
        /// Creates a QueryValue of a given type.
        /// </summary>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        public virtual QueryValue Visit(QueryReferenceType type)
        {
            throw new TaupoNotSupportedException("Not supported by generic converter. Use Entity Framework specific implementation.");
        }

        /// <summary>
        /// Wrapper method that performs basic null check and calls appropriate Accept method.
        /// </summary>
        /// <param name="value">Object that will be converted.</param>
        /// <param name="type">Type of the QueryValue that will be generated.</param>
        /// <returns>QueryValue representing the provided object.</returns>
        protected QueryValue Convert(object value, QueryType type)
        {
            if (value == null)
            {
                return type.NullValue;
            }
            else
            {
                this.ResultFragmentStack.Push(value);

                return type.Accept(this);
            }
        }
        
        private QueryStructuralValue PopulateProperties(QueryStructuralValue structuralToPopulate, object resultFragment)
        {
            foreach (var property in structuralToPopulate.Type.Properties)
            {
                var propertyInfo = resultFragment.GetType().GetProperty(property.Name);
                ExceptionUtilities.CheckObjectNotNull(propertyInfo, "Could not find appropriate property info for the given property: " + property.Name);

                var propertyValue = propertyInfo.GetValue(resultFragment, null);
                var propertyQueryValue = this.Convert(propertyValue, property.PropertyType);
                structuralToPopulate.SetValue(property.Name, propertyQueryValue);
            }

            return structuralToPopulate;
        }

        private QueryValue VisitScalar(QueryScalarType queryScalarType)
        {
            var scalarResult = this.ResultFragmentStack.Pop();
            
            return queryScalarType.CreateValue(scalarResult);
        }

        private QueryValue GenerateGroupingItem<TKey, TElement>(IGrouping<TKey, TElement> grouping, QueryGroupingType queryGroupingType)
        {
            var result = queryGroupingType.CreateNewInstance();
            var keyValue = this.Convert(grouping.Key, queryGroupingType.Key.PropertyType);
            var elementsValue = this.Convert(grouping.Select(g => g), queryGroupingType.Elements.PropertyType);

            result.SetValue("Key", keyValue);
            result.SetValue("Elements", elementsValue);

            return result;
        }
    }
}