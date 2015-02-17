//---------------------------------------------------------------------
// <copyright file="DataServiceProviderMethodsReplacementStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Method replacement strategy for the late-bound methods used with data service providers
    /// </summary>
    public abstract class DataServiceProviderMethodsReplacementStrategy : StaticMethodReplacementStrategyBase
    {
        /// <summary>
        /// Gets the data-service-provider-methods static class's type
        /// </summary>
        protected internal override Type MethodDeclarationType
        {
            get { return typeof(DataServiceProviderMethods); }
        }

        /// <summary>
        /// Gets the value of the given property
        /// </summary>
        /// <param name="value">The instance to get the property value from</param>
        /// <param name="resourceProperty">The property to get the value for</param>
        /// <returns>The property value</returns>
        public abstract object GetValue(object value, ResourceProperty resourceProperty);

        /// <summary>
        /// Converts the given value to the given type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="type">The type to convert to</param>
        /// <returns>The converted value</returns>
        public abstract object Convert(object value, ResourceType type);

        /// <summary>
        /// Returns whether or not the value is of the given type
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="type">The type to check</param>
        /// <returns>True if it is the given type, otherwise false</returns>
        public abstract bool TypeIs(object value, ResourceType type);

        /// <summary>
        /// Gets the value of a property that contains a sequence of items
        /// </summary>
        /// <typeparam name="T">The type of the sequence's elements</typeparam>
        /// <param name="value">The instance to get the property value from</param>
        /// <param name="resourceProperty">The property to get the value of</param>
        /// <returns>The value of the property</returns>
        public abstract IEnumerable<T> GetSequenceValue<T>(object value, ResourceProperty resourceProperty);

        /// <summary>
        /// Filters the given <paramref name="query"/> based on the <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="query">the query to filter.</param>
        /// <param name="resourceType">ResourceType based on which IQueryable needs to be filtered.</param>
        /// <param name="sourceType">The element type of the query before filtering.</param>
        /// <param name="resultType">The resulting query's element type.</param>
        /// <returns>an expression filtered by ResourceType.</returns>
        public abstract Expression OfType(Expression query, ResourceType resourceType, Type sourceType, Type resultType);

        /// <summary>Checks whether the given <paramref name="type"/> is assignable from the resource type of the <paramref name="value"/>.</summary>
        /// <typeparam name="T">Type representing the <paramref name="type"/></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="type">resource type instance against which the type as check needs to be performed.</param>
        /// <returns>null if the given value is not of the specified type, otherwise returns the value back.</returns>
        public abstract T TypeAs<T>(object value, ResourceType type);

        /// <summary>
        /// Tries to get a replacement expression for the given method with the given parameters
        /// </summary>
        /// <param name="toReplace">The method to replace</param>
        /// <param name="parameters">The parameters to the method</param>
        /// <param name="replaced">The replaced expression</param>
        /// <returns>True if a replacement was made, false otherwise</returns>
        public override bool TryGetReplacement(MethodInfo toReplace, IEnumerable<Expression> parameters, out Expression replaced)
        {
            if (this.CanReplace(toReplace) && toReplace.Name == "OfType")
            {
                var parameterExpressions = parameters.ToList();
                ExceptionUtilities.Assert(parameterExpressions.Count == 2, "OfType requires exactly 2 parameter expressions");
                
                var constantExpression = parameterExpressions[1] as ConstantExpression;
                ExceptionUtilities.CheckObjectNotNull(constantExpression, "OfType requires the 2nd parameter to be a constant");
                
                var resourceType = constantExpression.Value as ResourceType;
                ExceptionUtilities.CheckObjectNotNull(constantExpression, "OfType requires the 2nd parameter to be a resource type");
                
                var genericArguments = toReplace.GetGenericArguments();
                ExceptionUtilities.Assert(genericArguments.Length == 2, "OfType requires exactly 2 generic arguments");

                replaced = this.OfType(parameterExpressions[0], resourceType, genericArguments[0], genericArguments[1]);
                return true;
            }

            return base.TryGetReplacement(toReplace, parameters, out replaced);
        }

        /// <summary>
        /// Returns true unless the method is 'Compare', 'AreByteArraysEqual', or 'AreByteArraysNotEqual' which already have implementations
        /// or if it is 'OfType' which cannot be replaced with an instance method
        /// </summary>
        /// <param name="method">The method to consider replacing</param>
        /// <returns>True if it must be replaced, false otherwise</returns>
        protected internal override bool ShouldReplaceWithInstanceMethod(MethodInfo method)
        {
            return method.Name != "Compare" &&
                   method.Name != "AreByteArraysEqual" &&
                   method.Name != "AreByteArraysNotEqual" &&
                   method.Name != "OfType";
        }
    }
}