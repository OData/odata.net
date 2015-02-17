//---------------------------------------------------------------------
// <copyright file="DefaultDataServiceProviderMethodsReplacementStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Dictionary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;

    /// <summary>
    /// Default replacement strategy for the data-service-provider-methods late-bound expressions
    /// </summary>
    public class DefaultDataServiceProviderMethodsReplacementStrategy : DataServiceProviderMethodsReplacementStrategy
    {
        private IDataServiceQueryProvider queryProvider;

        /// <summary>
        /// Initializes a new instance of the DefaultDataServiceProviderMethodsReplacementStrategy class
        /// </summary>
        /// <param name="queryProvider">The current query provider</param>
        public DefaultDataServiceProviderMethodsReplacementStrategy(IDataServiceQueryProvider queryProvider)
        {
            this.queryProvider = queryProvider;
        }

        /// <summary>
        /// Gets the value of the given property
        /// </summary>
        /// <param name="value">The instance to get the property value from</param>
        /// <param name="resourceProperty">The property to get the value for</param>
        /// <returns>The property value</returns>
        public override object GetValue(object value, ResourceProperty resourceProperty)
        {
            return this.queryProvider.GetPropertyValue(value, resourceProperty);
        }

        /// <summary>
        /// Converts the given value to the given type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="type">The type to convert to</param>
        /// <returns>The converted value</returns>
        public override object Convert(object value, ResourceType type)
        {
            if (this.TypeIs(value, type))
            { 
                return value;
            }
             
            return null;
        }

        /// <summary>
        /// Returns whether or not the value is of the given type
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="type">The type to check</param>
        /// <returns>True if it is the given type, otherwise false</returns>
        public override bool TypeIs(object value, ResourceType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            // Projections require DataServiceProviderMethods.TypeIs to be implemented even if all resource types have CanReflect=true
            // when bug is fixed, uncomment the following line
            //// ExceptionUtilities.Assert(!type.CanReflectOnInstanceType, "DataServiceProviderMethods.TypeIs should not be used when CanReflectOnInstanceType = true");

            if (value == null)
            {
                return false;
            }

            ResourceType instanceType = this.GetResourceType(value);
            if (instanceType != null)
            {
                return IsAssignable(type, instanceType);
            }

            return type.InstanceType.IsAssignableFrom(value.GetType());
        }

        /// <summary>
        /// Gets the value of a property that contains a sequence of items
        /// </summary>
        /// <typeparam name="T">The type of the sequence's elements</typeparam>
        /// <param name="value">The instance to get the property value from</param>
        /// <param name="resourceProperty">The property to get the value of</param>
        /// <returns>The value of the property</returns>
        public override IEnumerable<T> GetSequenceValue<T>(object value, ResourceProperty resourceProperty)
        {
            var list = (IEnumerable)this.GetValue(value, resourceProperty);

            if (typeof(IEnumerable<T>).IsAssignableFrom(list.GetType()))
            {
                return (IEnumerable<T>)list;
            }

            return list.Cast<T>();
        }

        /// <summary>
        /// Filters the given <paramref name="query"/> based on the <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="query">the query to filter.</param>
        /// <param name="resourceType">ResourceType based on which IQueryable needs to be filtered.</param>
        /// <param name="sourceType">The element type of the query before filtering.</param>
        /// <param name="resultType">The resulting query's element type.</param>
        /// <returns>an expression filtered by ResourceType.</returns>
        public override Expression OfType(Expression query, ResourceType resourceType, Type sourceType, Type resultType)
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");
            ExceptionUtilities.CheckArgumentNotNull(resourceType, "resourceType");
            ExceptionUtilities.CheckArgumentNotNull(sourceType, "sourceType");
            ExceptionUtilities.CheckArgumentNotNull(resultType, "resultType");
            ExceptionUtilities.Assert(!resourceType.CanReflectOnInstanceType, "DataServiceProviderMethods.OfType should not be used when CanReflectOnInstanceType = true");

            var parameter = Expression.Parameter(sourceType, "o");
            var typeIsExpression = Expression.Call(Expression.Constant(this), "TypeIs", new Type[0], parameter, Expression.Constant(resourceType));
            var lambdaExpression = Expression.Lambda(typeIsExpression, parameter);

            if (typeof(IQueryable).IsAssignableFrom(query.Type))
            {
                return Expression.Call(typeof(Queryable), "Cast", new[] { resultType }, Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, query, lambdaExpression));
            }
            else
            {
                return Expression.Call(typeof(Enumerable), "Cast", new[] { resultType }, Expression.Call(typeof(Enumerable), "Where", new[] { sourceType }, query, lambdaExpression));
            }   
        }

        /// <summary>Checks whether the given <paramref name="type"/> is assignable from the resource type of the <paramref name="value"/>.</summary>
        /// <typeparam name="T">Type representing the <paramref name="type"/></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="type">resource type instance against which the type as check needs to be performed.</param>
        /// <returns>null if the given value is not of the specified type, otherwise returns the value back.</returns>
        public override T TypeAs<T>(object value, ResourceType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.Assert(!type.CanReflectOnInstanceType, "DataServiceProviderMethods.TypeAs should not be used when CanReflectOnInstanceType = true");

            if (value == null)
            {
                return default(T);
            }

            if (this.TypeIs(value, type))
            {
                return (T)value;
            }

            return default(T);
        }

        /// <summary>
        /// Helper method used for checking inheritance.
        /// </summary>
        /// <param name="type">The type to compare against</param>
        /// <param name="instanceType">The instance type</param>
        /// <returns>Whether or not the instance type is assignable to the given type</returns>
        private static bool IsAssignable(ResourceType type, ResourceType instanceType)
        {
            do
            {
                if (type == instanceType)
                {
                    return true;
                }

                instanceType = instanceType.BaseType;
            }
            while (instanceType != null);

            return false;
        }

        private ResourceType GetResourceType(object instance)
        {
            ResourceType type = null;
            ProviderImplementationSettings.Override(
                s => s.EnforceMetadataCaching = false,
                () => type = this.queryProvider.GetResourceType(instance));
            return type;
        }
    }
}